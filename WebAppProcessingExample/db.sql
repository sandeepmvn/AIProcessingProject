-- create a database named as "Processingdb"
CREATE DATABASE Processingdb;

-- use the database
USE Processingdb;

-- create a table named as "AnalysisProjecttbl" COlumns are PKId string, Name varchar(50), ImageURL varchar(1000), IsProcessed bit
CREATE TABLE AnalysisProjecttbl
(
	PKId varchar(15),
	Name VARCHAR(50),
	ImageURL VARCHAR(1000),
	IsProcessed BIT,
	Result VARCHAR(MAX)
);

