#!/usr/bin/env bash

  sudo bash

  
# install mysql
 
 debconf-set-selections <<< 'mysql-server-5.5 mysql-server/root_password password bob'

  debconf-set-selections <<< 'mysql-server-5.5 mysql-server/root_password_again password bob'


  apt-get -y install mysql-server


  # setup remote access

  mysql -u root -pbob < /vagrant/up/setrootprivs.sql

  service mysql restart

  mysql -u root -pbob < /vagrant/up/remoteusers.sql


  mysql -u root -pbob < /vagrant/up/agseed30-2014-12-30.sql


  # allow remote connections from all hosts

  sed s/127.0.0.1/0.0.0.0/ /etc/mysql/my.cnf > my.cnf

  mv my.cnf /etc/mysql/my.cnf

  service mysql restart
