#!/usr/bin/env Rscript

# Creates or updates a NFL play-by-play database in a given location
# Rscript UpdateDB.R dbname host port username pw years(yyyy,yyyy,... or empty for rebuild all)

args = commandArgs(trailingOnly=TRUE)
DBNAME <- args[1]                                           
HOST <- args[2]                                             
PORT <- args[3]                                             
USERNAME <- args[4]                                         
PASSWORD <- args[5]                                         
if(!is.na(args[6])) {
  YEARS <- unlist(lapply(strsplit(args[6], ","), as.numeric)) # 2021,2022
} else {
  YEARS = TRUE
}

future::plan("multisession")

library(nflfastR)
library(DBI)
library(RPostgres)

# Create db conn
db_connection <- dbConnect(
  drv = Postgres(),
  dbname = DBNAME,
  host = HOST,
  port = PORT,
  user = USERNAME,
  password = PASSWORD,
)

nflfastR::update_db(
  db_connection = db_connection,
  tblname = 'nfl_pbp',
  force_rebuild = YEARS,
                    )

dbDisconnect(db_connection)
