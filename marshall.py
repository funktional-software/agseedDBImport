__author__ = 'mikba'
import MySQLdb
import pyodbc

##############################################################
# get connections to MySQL server hosting old db
##############################################################
db = MySQLdb.connect(host='localhost', user='root',  passwd='bob', db='agseed30')
mysqlcursor = db.cursor()

##############################################################
# get connection to SQLServer hosting new db
##############################################################
cnxn = pyodbc.connect('DRIVER={SQL Server};SERVER=192.168.1.109;DATABASE=agSeedSelect;UID=sa;PWD=a')
sqlsrvrcursor = cnxn.cursor()





##############################################################
# marshall products
##############################################################
for row in mysqlcursor.execute("select * from products").fetchall():
	print row[2]


##############################################################
# marshall characteristics
##############################################################



##############################################################
# marshall users
##############################################################



##############################################################
# marshall guides
##############################################################



