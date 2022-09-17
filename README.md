# Projects
<p>This portfolio will be a compilation of notebooks, dashboards and other standalone projects which I have created for data analysis, exploring machine learning or a specific application. The projects are divided by category.</p>

## Power BI Dashboards

### NFL Analysis

[View Dashboard](https://app.powerbi.com/view?r=eyJrIjoiYmMyYWY2ZjgtNGM1ZC00ZGVjLWFhODMtYTY5OTM0N2I1YmJmIiwidCI6IjhkZWQ3ODVjLTJiYTYtNGIxYS05NmUyLWY3NGFiZTk2MWFiZCIsImMiOjh9)
<br><br>
![NFL Dashboard game view](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/NFL/NFL_1.PNG?raw=true "Game view")

This dashboard offers functionality for comparing NFL team performance and play type (e.g. running play, passing play) likelihood and yard gains.
Additionally, it allows for a detailed view of any NFL game from 1999 to present day.
Functionality for predicting the outcome of a single play with full parameter control is planned, but not yet at a working state.<br>
<br>
This project uses data provided by [nflfastR](https://www.nflfastr.com/index.html), a R package for obtaining NFL play-by-play data.<br>
At the moment, a SQLite database is built and updated locally by the package and afterwards the dashboard is refreshed from the updated database.<br>

The learning goal of this project was to learn how to use Power BI for Dashboard creation, using a big datasource that needs a significant amount of preprocessing work in Power Query. Many of the visuals heavily utilize custom DAX measures both for learning purposes but also as the dataset structure necessitated this - the improved performance is an added bonus!

## Standalone projects

### "LiHy's Little Helper" - Telegram bot for event submissions (Python)

[View Code](https://github.com/epaunonen/epaunonen.github.io/tree/main/Projects/LiHy's%20Little%20Helper)
<br><br>
![LiHyBot](https://github.com/epaunonen/epaunonen.github.io/blob/main/Assets/LiHyBot/LLH.PNG?raw=true "Telegram Bot")

This bot was created for a student-run wellbeing event that ran from June to August 2022. The idea of the event was to make the participant document their summer through pictures. A Telegram chatbot was chosen as a submission platform as it is easily available for everyone, quick to use for picture submissions and allows for a neat UI implementation.
<br><br>
The bot was developed using Python with [python-telegram-bot](https://github.com/python-telegram-bot/python-telegram-bot) as a wrapper for the Telegram bot api. As a feature request by the event organizers the bot used Google Drive, with drive api through a specifically created Google Service Account, as storage for the submissions. This was done to allow the organizers to conveniently access the pictures for verification. The bot was deployed to an Amazon EC2 instance, which was configured to automatically restart the process if a critical error occurred - a failsafe that was not triggered at all during the event.

**Stats:**
 - 55 unique users
 - 542 pictures submitted in total
 - 71 days

**Things I would do differently next time:**
 - Error handling: As the bot was utilized by relatively few people and put together very quickly and the data sent through the bot was not deemed critical, I did not thorougly prepare for errors. I made a decision that if a picture submission failed, the bot would forward the picture as well as the error message to a specified admin account (=me). After that I would manually submit the picture again. This ended up working fine as only 9 pictures encountered an error (1.7% of total), but for a larger application this would be unacceptable. It should be noted that all these errors were caused by the Telegram API not responding even after two retries.
 - Consider not using Google Drive for storage, as the api is cumbersome and quite slow for an application like this.
