import datetime
import sys
from urllib.request import urlopen

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

def run(args):
    if len(args) < 9: return print('Incorrect arguments specified, specify:\nhost, port, dbname, user, password, table_name,\nseasons\n(a=all, c=current, or specify years yyyy,...),\noperation logic (d=delete, r=delete and rebuild, u=update)')
    
    # get params from arguments
    HOST = args[1]
    PORT = args[2]
    DBNAME = args[3]
    USER = args[4]
    PASSWORD = args[5]
    TABLE_NAME = args[6]
    
    # seasons
    if args[7] == 'a':
        curr_year = datetime.date.today().year
        if datetime.date.today() >= datetime.date(curr_year, 9, 1): 
            # current year season has started
            lim = curr_year + 1
        else: 
            # current year season has not yet started
            lim = curr_year 
        SEASONS = list(range(1999, lim))
    
    elif args[7] == 'c':
        curr_year = datetime.date.today().year
        if datetime.date.today() >= datetime.date(curr_year, 9, 1): 
            SEASONS = [curr_year]
        else:
            SEASONS = [curr_year - 1]
    
    else: SEASONS = [int(i) for i in args[7].split(',')]
    
    # logic
    if args[8] == 'd':
        LOGIC = 'D'
        print('Logic: Delete data')
    elif args[8] == 'r':
        LOGIC = 'DR'
        print('Logic: Clean rebuild')
    elif args[8] == 'u':
        LOGIC = 'R'
        print('Logic: Update diff')
        raise Exception('u Not implemented yet!')
    else: raise Exception("Incorrect operation logic specified, use 'd', 'r' or 'u'!")
    
    rebuild(host=HOST, port=PORT, dbname=DBNAME, user=USER, password=PASSWORD, table_name=TABLE_NAME, seasons=SEASONS, logic=LOGIC)
     
def main():
    run(sys.argv)
    
if __name__ == '__main__':
    main()