import json
import datetime
from urllib.request import urlopen

#from layer
import pg8000.dbapi as pg


def purgeSeasons(cur : pg.Cursor, table_name : str, seasons : list):
    s = '(' + ','.join(map(str, seasons)) + ')'
    cur.execute('DELETE FROM {} WHERE season IN {}'.format(table_name, s))
    
def addSeason(cur : pg.Cursor, table_name : str, season : int):
    
    cur.execute('CREATE TEMP TABLE tmp_table ON COMMIT DROP AS SELECT * FROM {} WITH NO DATA'.format(table_name))
    
    url = 'https://github.com/nflverse/nflverse-data/releases/download/pbp/play_by_play_{}.csv'.format(season)
    with urlopen(url) as response:
        file = [l.decode('utf-8') for l in response.readlines()]

        cur.execute('COPY tmp_table FROM STDIN WITH (FORMAT CSV)', stream=file[1:])
        print('Inserted {} rows.'.format(len(file)-1))
                
        cur.execute('INSERT INTO {} SELECT * FROM tmp_table ON CONFLICT DO NOTHING'.format(table_name))
        cur.execute('DROP TABLE tmp_table')
                
    
def rebuild(host : str, port : str, dbname : str, user : str, password : str, table_name : str, seasons : list, logic : str):
    
    with pg.connect(host=host, port=port, database=dbname, user=user, password=password) as conn:
        
        # Create cursor
        cur = conn.cursor()
           
        # Delete data from selected seasons
        if 'D' in logic:
            print('Purging data from season(s) {}...'.format(seasons))
            purgeSeasons(cur, table_name, seasons)
            print('===== Complete =====\n')
            
        # Download and add data from selected seasons to the db
        if 'R' in logic:
            print('Downloading data...')
            for season in seasons:
                print('==== Season {} ==='.format(season))
                addSeason(cur, table_name, season)
                print('\n')
            
        conn.commit()
        print('===== Complete =====')

    
def lambda_handler(event, context):
    
    HOST = event['host']
    PORT = event['port']
    DBNAME = event['dbname']
    USER = event['user']
    PASSWORD = event['password']
    TABLE_NAME = event['tablename']
    
    # seasons
    if event['seasons'] == 'a':
        curr_year = datetime.date.today().year
        if datetime.date.today() >= datetime.date(curr_year, 9, 1): 
            # current year season has started
            lim = curr_year + 1
        else: 
            # current year season has not yet started
            lim = curr_year 
        SEASONS = list(range(1999, lim))
    
    elif event['seasons'] == 'c':
        curr_year = datetime.date.today().year
        if datetime.date.today() >= datetime.date(curr_year, 9, 1): 
            SEASONS = [curr_year]
        else:
            SEASONS = [curr_year - 1]
    
    else: SEASONS = [int(i) for i in event['seasons'].split(',')]
    
    # logic
    if event['logic'] == 'd':
        LOGIC = 'D'
        print('Logic: Delete data')
    elif event['logic'] == 'r':
        LOGIC = 'DR'
        print('Logic: Clean rebuild')
    elif event['logic'] == 'u':
        LOGIC = 'R'
        print('Logic: Update diff')
        raise Exception('u Not implemented yet!')
    else: raise Exception("Incorrect operation logic specified, use 'd', 'r' or 'u'!")
    
    rebuild(host=HOST, port=PORT, dbname=DBNAME, user=USER, password=PASSWORD, table_name=TABLE_NAME, seasons=SEASONS, logic=LOGIC)
    
    response = {
        'message': 'Data refreshed successfully'
    }
    return {
        'statusCode': 200,
        'body': response
    }