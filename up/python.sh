#!/usr/bin/env bash

  sudo bash

  apt-get update
  apt-get install -y python-dev
  apt-get install -y libmysqlclient-dev
  apt-get install -y python-mysqldb
  apt-get install -y unixodbc-dev
  apt-get -y install python-pip
  pip install --allow-external pyodbc --allow-unverified pyodbc pyodbc




