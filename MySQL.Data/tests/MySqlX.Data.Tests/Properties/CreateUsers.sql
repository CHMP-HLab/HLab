DROP USER IF EXISTS 'test'@'localhost';
DROP USER IF EXISTS 'testNoPass'@'localhost';
DROP USER IF EXISTS 'testSha256'@'localhost';
CREATE USER 'test'@'localhost' identified by 'test';
GRANT ALL PRIVILEGES  ON *.*  TO 'test'@'localhost';
CREATE USER 'testNoPass'@'localhost';
CREATE USER 'testSha256'@'localhost' identified with sha256_password by 'mysql';
GRANT ALL PRIVILEGES  ON *.*  TO 'testSha256'@'localhost';
FLUSH PRIVILEGES;



