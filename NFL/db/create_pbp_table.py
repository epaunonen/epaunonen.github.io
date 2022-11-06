import psycopg as pg
import sys

def get_setup_string():
    s = ''
    file = open('dbsetup.txt')
    for line in file:
        splt = line.split('|')  
        for st in splt:
            st = st.strip()
            if st != '': st = ' ' + st
            s += st
        s += ','
    s = s[:-1]    
    file.close()
    return s

def create(host : str, port : str, dbname : str, user : str, password : str, table_name : str, mode = None):
    # Connect to db
    with pg.connect("host='{}' port='{}' dbname='{}' user='{}' password='{}'".format(host, port, dbname, user, password)) as conn:
        
        s = 'CREATE TABLE {} ({})'.format(table_name, get_setup_string())
        
        with conn.cursor() as cur:
            
            # check if table with given name exists
            cur.execute("SELECT EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = '{}')".format(table_name))
            res = cur.fetchall()
            if res[0][0]: # exists
                if mode == 'f':
                    print('Dropping existing table...')
                    cur.execute('DROP TABLE {}'.format(table_name))
                    conn.commit()
                else:
                    return print('Table with given name already exists in the database, please specify argument f if wanting to delete and recreate.')
            
            print('Creating table...')
            cur.execute(s)
        
        conn.commit()
        print('Done!')

def init_index(host : str, port : str, dbname : str, user : str, password : str, table_name : str):
    pass

def run(args):
    if len(args) < 7: return print('Incorrect arguments specified, needs:\nhost, port, dbname, user, password, table_name, (optional) f=force rebuild, i=create indexes')
    
    # get params from arguments
    HOST = args[1]
    PORT = args[2]
    DBNAME = args[3]
    USER = args[4]
    PASSWORD = args[5]
    TABLE_NAME = args[6]
    MODE = None
    
    if len(args) == 8:
        if args[7] == 'f':
            MODE = 'f'
        elif args[7] == 'i':
            init_index(host=HOST, port=PORT, dbname=DBNAME, user=USER, password=PASSWORD, table_name=TABLE_NAME)
            return
            
    create(host=HOST, port=PORT, dbname=DBNAME, user=USER, password=PASSWORD, table_name=TABLE_NAME, mode=MODE)
    
def main():
    run(sys.argv)
    
if __name__ == '__main__':
    main()