

CREATE USER 'vagrem'@'localhost' IDENTIFIED BY 'vagpass';
CREATE USER 'vagrem'@'%' IDENTIFIED BY 'vagpass';

GRANT ALL ON *.* TO 'vagrem'@'localhost';
GRANT ALL ON *.* TO 'vagrem'@'%';
flush privileges;