import logging
from functools import wraps
from telegram import (KeyboardButton, ReplyKeyboardRemove, Update, ReplyKeyboardMarkup)
import telegram
from telegram.ext import (ApplicationBuilder, CallbackContext, CommandHandler, MessageHandler, filters)
import telegram.error
import util
from io import BytesIO
from googleapiclient.http import MediaIoBaseUpload
import drivepy
import os
import signal



API_KEY = '' #Telegram Bot API key
LIST_OF_OWNERS = [] #Telegram user ID
LIST_OF_ADMINS = []
MAIN_FOLDER_ID = '' #Google Drive parent folder ID
protection = True

submit_texts = ["Food", "Drink", "Sports", "Summer Activities", "Optional"]

service = None
application = None

logging.basicConfig(
    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
    level=logging.INFO
)
logger = logging.getLogger(__name__)

# Decorators
def admins(func):
    @wraps(func)
    async def wrapped(update, context, *args, **kwargs):
        user_id = update.effective_user.id
        if user_id not in LIST_OF_ADMINS and user_id not in LIST_OF_OWNERS:
            print(f"Unauthorized access denied for {user_id}.")
            return
        return await func(update, context, *args, **kwargs)
    return wrapped

def owners(func):
    @wraps(func)
    async def wrapped(update, context, *args, **kwargs):
        user_id = update.effective_user.id
        if user_id not in LIST_OF_OWNERS:
            print(f"Unauthorized access denied for {user_id}.")
            return
        return await func(update, context, *args, **kwargs)
    return wrapped

# funcs

async def start(update: Update, context: CallbackContext.DEFAULT_TYPE):
    await context.bot.send_message(chat_id=update.effective_chat.id, text="<b>Great to have you on board!</b>\n\nNow let’s have a quick tour of what I can do.\n\nSelect “<b>Menu</b>” from the left corner to\n <b>1.</b>  Submit a new photo\n <b>2.</b>  Show your submissions in each category", parse_mode=telegram.constants.ParseMode.HTML)

        
async def stats(update: Update, context: CallbackContext):
    
    try:
        files = await get_files(update, context)
    except:
        global service
        service = create_service()
        try:
            files = await get_files(update, context)
        except:
            await context.bot.send_message(chat_id=update.effective_chat.id, text='I\'m having some trouble retrieving your stats at this moment :( Don\'t worry, your pictures are still safe and sound!')
            await log_error('LOG_ERROR: Failed to retrieve stats after recreating service')
            return
    
    if len(files) == 0:
        await context.bot.send_message(chat_id=update.effective_chat.id, text='You haven\'t submitted anything yet!\nUse /submit to send me your first summer picture!')
        return
    
    prefixes = []
    for file in files:
        prefixes.append(file.rsplit(' ', 1)[0])
        
    rstring = "<b>Great job! Here are your submissions in each category:</b>\n\n"
    for category in submit_texts:
        n = prefixes.count(category)
        rstring += category + ': <b>' + str(n) + '</b>\n'

    await context.bot.send_message(chat_id=update.effective_chat.id, text=rstring, parse_mode=telegram.constants.ParseMode.HTML)
    return
    

async def get_files(update: Update, context: CallbackContext):
    
    # Find user folder
    page_token = None
    folders = []
    while True:
        response = service.files().list(q='mimeType="application/vnd.google-apps.folder" and "{}" in parents and properties has {{ key="oid" and value="{}" }}'.format(MAIN_FOLDER_ID, update.effective_user.id),
                                        fields='nextPageToken, files(name, id)',
                                        pageToken=page_token).execute()
        for file in response.get('files', []):
            folders.append(file.get('id'))
        page_token = response.get('nextPageToken', None)
        if page_token is None:
            break
    
    if len(folders) <= 0:
        return []
    
    folder_id = folders[0]
    
    # Get files in user folder
    page_token = None
    files = []
    while True:
        response = service.files().list(q='mimeType != "application/vnd.google-apps.folder" and "{}" in parents'.format(folder_id),
                                        fields='nextPageToken, files(name, id)',
                                        pageToken=page_token).execute()
        for file in response.get('files', []):
            files.append(file.get('name'))
        page_token = response.get('nextPageToken', None)
        if page_token is None:
            break
    return files
 

async def submit(update: Update, context: CallbackContext):
    button_list = []
    for text in submit_texts:
        button_list.append(KeyboardButton(text))
    
    reply_markup = ReplyKeyboardMarkup(util.build_menu(button_list, n_cols=3), one_time_keyboard=True)
    await context.bot.send_message(chat_id=update.effective_chat.id, text='Choose the category of your summer picture \U0001F349', reply_markup=reply_markup, parse_mode=telegram.constants.ParseMode.HTML)      
      
        
async def submit_message(update: Update, context: CallbackContext.DEFAULT_TYPE):
    if update.message.text in submit_texts:
        reply_markup = ReplyKeyboardRemove()
        
        context.user_data['imagetype'] = update.message.text
        
        await context.bot.send_message(chat_id=update.effective_chat.id, text='<b>Ink</b>redible \U0001F419 Now send me the photo!\n\n<i>Wrong category?\nSimply press or type /submit to resubmit.</i>', reply_markup=reply_markup, parse_mode=telegram.constants.ParseMode.HTML)
    else:
        await context.bot.send_message(chat_id=update.effective_chat.id, text='Sorry, I can’t understand you \U0001F97A', parse_mode=telegram.constants.ParseMode.HTML)
 

async def photo(update: Update, context: CallbackContext):
    imagetype = context.user_data.get('imagetype', '')
    if 'imagetype' in context.user_data: del context.user_data['imagetype']
    
    if imagetype == '':
        await context.bot.send_message(chat_id=update.effective_chat.id, text='Please use <i>/submit</i> to begin submitting a photo and follow the instructions.', parse_mode=telegram.constants.ParseMode.HTML)
    else:
        
        mid = update.message.message_id
        chatid = update.effective_chat.id
        userid = update.effective_user.id
        
        await context.bot.send_message(chat_id=update.effective_chat.id, text='Thanks for your submission!\nUse <i>/submit</i> to send me more pictures!', parse_mode=telegram.constants.ParseMode.HTML)      
        try:
            #raise Exception()
            await upload(update, context, imagetype)
        except: # create new service and retry
            global service
            service = create_service()
            try:
                #raise Exception()
                await upload(update, context, imagetype)
            except: 
                await log_error('LOG_ERROR: Failed to upload image. UserId: {} - {}'.format(userid, imagetype))
                await context.bot.forward_message(chat_id=LIST_OF_OWNERS[0], from_chat_id=chatid, message_id=mid, connect_timeout=30, write_timeout=60, read_timeout=60)


async def upload(update: Update, context: CallbackContext, imagetype: str):
    # Query for folder containing user id in name in main folder
    page_token = None
    folder_ids = []
    while True:
        response = service.files().list(q='mimeType="application/vnd.google-apps.folder" and "{}" in parents and properties has {{ key="oid" and value="{}" }}'.format(MAIN_FOLDER_ID, update.effective_user.id),
                                        fields='nextPageToken, files(id)',
                                        pageToken=page_token).execute()
        for file in response.get('files', []):
            folder_ids.append(file.get('id'))
        page_token = response.get('nextPageToken', None)
        if page_token is None:
            break
    
    folder_id = None
    
    # Returned zero matches, create new folder
    if len(folder_ids) == 0:
        folder_metadata = {
            'name': '{} - {}'.format(update.effective_user.username, update.effective_user.full_name),
            'mimeType': 'application/vnd.google-apps.folder',
            'parents': [MAIN_FOLDER_ID],
            'description': '{}'.format(update.effective_user.id),
            'properties': {'oid': '{}'.format(update.effective_user.id)},
        }
        newfolder = service.files().create(body=folder_metadata, fields='id').execute()
        folder_id = newfolder.get('id')
    
    # Match found, assume folder_id[0] to be the correct folder and disregard other matches
    else:
        folder_id = folder_ids[0]
        
    # Process and upload the photo into user folder, has a risk of timeout!?
    f = await update.message.effective_attachment[-1].get_file(read_timeout=60, connect_timeout=30)
    image = BytesIO(await f.download_as_bytearray())
    
    media = MediaIoBaseUpload(image, mimetype='image/jpeg')
    file = service.files().create(
        media_body = media,
        body={'name' : '{} {}'.format(imagetype, f.file_unique_id), 'parents': [folder_id], 'properties': {'oid': '{}'.format(update.effective_user.id), 'nick': '{}'.format(update.effective_user.username)}}, 
        fields='id'
    ).execute()
    
# Send error notification to owner, stats, upload, restart
async def log_error(text: str, image_id=None, image_text=''):
    for owner in LIST_OF_OWNERS:
        await application.bot.send_message(chat_id=owner, text=text, write_timeout=60, connect_timeout=30)
        if image_id is not None:
            await application.bot.send_photo(chat_id=owner, photo=image_id, caption=image_text, write_timeout=60, connect_timeout=30) 
        
# Admin

@owners
async def unprotect(update: Update, context: CallbackContext):
    global protection
    protection= False
    await context.bot.send_message(chat_id=update.effective_chat.id, text='Protection is now disabled.')

@owners
async def protect(update: Update, context: CallbackContext):
    global protection    
    protection = True
    await context.bot.send_message(chat_id=update.effective_chat.id, text='Protection is now enabled.')

@owners
async def emptydrive(update: Update, context: CallbackContext):
    # DELETE EVERYTHING OWNED BY THE SERVICE ACCOUNT, SKIPPING TRASH
    global protection
    if protection:
        await context.bot.send_message(chat_id=update.effective_chat.id, text='Could not complete operation, protection is enabled.')
        return
    
    # Find all files owned
    page_token = None
    file_ids = []
    while True:
        response = service.files().list(q='"me" in owners',
                                        fields='nextPageToken, files(id)',
                                        pageToken=page_token).execute()
        for file in response.get('files', []):
            file_ids.append(file.get('id'))
        page_token = response.get('nextPageToken', None)
        if page_token is None:
            break
        
    try:
        for id in file_ids:
            service.files().delete(fileId=id).execute()
    except:
        await log_error('LOG_ERROR: Encountered an error while emptying drive')
    
    protection = True
    await context.bot.send_message(chat_id=update.effective_chat.id, text='ADMIN COMMAND ISSUED: Deleted all drive data, {} files have been deleted'.format(len(file_ids)))

@owners
async def broadcast(update: Update, context: CallbackContext):
    global protection
    if protection:
        await context.bot.send_message(chat_id=update.effective_chat.id, text='Could not complete operation, protection is enabled.')
        return
    #NOT IMPLEMENTED
    protection = True

    
async def general_error(update: Update, context: CallbackContext):
    #print(context.error)
    await log_error('Encountered an unspecified error: {}'.format(context.error))
        
@owners
async def kill(update: Update, context:CallbackContext):
    await log_error('Killing process..')
    os.kill(os.getpid(), signal.SIGKILL)
    
    
def create_service():
    scope = 'https://www.googleapis.com/auth/drive'
    key_file_location = 'nice-incline.json'
    return drivepy.get_service(
        api_name = 'drive',
        api_version = 'v3',
        scopes = [scope],
        key_file_location = key_file_location,
    )

def main() -> None:
    
    # Authenticate to Drive
    global service
    service = create_service()
    
    global application
    application = ApplicationBuilder().token(API_KEY).build()
    
    # Create command handlers
    start_handler = CommandHandler('start', start)
    submit_handler = CommandHandler('submit', submit)
    stats_handler = CommandHandler('stats', stats)
    submit_message_handler = MessageHandler(filters.TEXT & (~filters.COMMAND), submit_message)
    photo_handler = MessageHandler(filters.PHOTO, photo)
    
    protect_handler = CommandHandler('protect', protect)
    unprotect_handler = CommandHandler('unprotect', unprotect)
    emptydrive_handler = CommandHandler('emptydrive', emptydrive)
    kill_handler = CommandHandler('kill', kill)
    
    # Add command handlers
    application.add_handler(start_handler)
    application.add_handler(submit_message_handler)
    application.add_handler(submit_handler)
    application.add_handler(stats_handler)
    application.add_handler(photo_handler)
    
    application.add_handler(protect_handler)
    application.add_handler(unprotect_handler)
    application.add_handler(emptydrive_handler)
    application.add_handler(kill_handler)
    
    application.add_error_handler(general_error)
    
    application.run_polling(stop_signals=None)
    


    
if __name__ == '__main__':  
    main()
    

