import datetime
import sys
import psycopg as pg
from urllib.request import urlopen
import csv

def purgeSeasons(cur : pg.Cursor, table_name : str, seasons : list):
    s = '(' + ','.join(map(str, seasons)) + ')'
    cur.execute('DELETE FROM {} WHERE season IN {}'.format(table_name, s))
    
def addSeason(cur : pg.Cursor, table_name : str, season : int):
    url = 'https://github.com/nflverse/nflverse-data/releases/download/pbp/play_by_play_{}.csv'.format(season)
    with urlopen(url) as response:
        lines = [l.decode('utf-8') for l in response.readlines()]
        file = csv.reader(lines)
        
        with cur.copy('COPY {} FROM STDIN'.format(table_name)) as copy:
            rc = 0
            
            for row in [row for row in file][1:]:
                rc += 1
                print('   Row {}'.format(rc), end='\r', flush=True)
                
                for i in range(len(row)):
                    if row[i] == '': row[i] = None
                
                copy.write_row(row) 
    
def rebuild(host : str, port : str, dbname : str, user : str, password : str, table_name :str, seasons : list):
    
    # Connect to db
    with pg.connect("host='{}' port='{}' dbname='{}' user='{}' password='{}'".format(host, port, dbname, user, password)) as conn:
        
        # Create cursor
        with conn.cursor() as cur:
           
            # Delete data from selected seasons
            print('Purging data from season(s) {}...'.format(seasons))
            purgeSeasons(cur, table_name, seasons)
            print('===== Complete =====\n\nDownloading data...')
               
            # Download and add data from selected seasons to the db
            for season in seasons:
                print('==== Season {} ==='.format(season))
                addSeason(cur, table_name, season)
                print('\n')
            
        conn.commit()
        print('===== Complete =====')
            
def main():
    if len(sys.argv) < 8: return print('Incorrect arguments specified, specify host, port, dbname, user, password, table_name and seasons (yyyy,..., a for all or c for current')
    
    # get params from command line arguments
    HOST = sys.argv[1]
    PORT = sys.argv[2]
    DBNAME = sys.argv[3]
    USER = sys.argv[4]
    PASSWORD = sys.argv[5]
    TABLE_NAME = sys.argv[6]
    
    if sys.argv[7] == 'a':
        curr_year = datetime.date.today().year
        if datetime.date.today() >= datetime.date(curr_year, 9, 1): 
            # current year season has started
            lim = curr_year + 1
        else: 
            # current year season has not yet started
            lim = curr_year 
        SEASONS = list(range(1999, lim))
    
    elif sys.argv[7] == 'c':
        curr_year = datetime.date.today().year
        if datetime.date.today() >= datetime.date(curr_year, 9, 1): 
            SEASONS = [curr_year]
        else:
            SEASONS = [curr_year - 1]
    
    else: SEASONS = [int(i) for i in sys.argv[7].split(',')]
    
    rebuild(host=HOST, port=PORT, dbname=DBNAME, user=USER, password=PASSWORD, table_name=TABLE_NAME, seasons=SEASONS)
    
if __name__ == '__main__':
    main()