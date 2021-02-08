05 Feb 2021
===========

Use PostgreSQL + PostGIS
------------------------

Import GeoNames data from geonames.org .. allCountries.zip

postgres=# create database geonames;
CREATE DATABASE
postgres=# \c geonames
You are now connected to database "geonames" as user "postgres".
geonames=# create extension postgis;
CREATE EXTENSION
geonames=# select postgis_full_version();
 

geonames=# create table allcountries (countrycode varchar(2), postalcode varchar(20), placename varchar(180), aname1 varchar(100), acode1 varchar(20), aname2 varchar(100), acode2 varchar(20), aname3 varchar(100), acode3 varchar(20), latitude float, longitude float, accuracy integer);
CREATE TABLE
geonames=# copy allcountries from 'C:\Prasun\Office\Network Planner\R1.2.2\ServerSideClustering - MCP\Data\allCountries.txt' delimiter E'\t' CSV;
ERROR:  character with byte sequence 0x8f in encoding "WIN1252" has no equivalent in encoding "UTF8"
CONTEXT:  COPY allcountries, line 56186
geonames=# SET CLIENT_ENCODING TO 'utf8';
SET
geonames=# copy allcountries from 'C:\Prasun\Office\Network Planner\R1.2.2\ServerSideClustering - MCP\Data\allCountries.txt' delimiter E'\t' CSV;
 - Some entries were giving errors, so i removed them from the file
geonames=# copy allcountries from 'C:\Prasun\Office\Network Planner\R1.2.2\ServerSideClustering - MCP\Data\allCountries.txt' delimiter E'\t' CSV;
COPY 1479415
geonames=#   
geonames=# alter table allcountries add column geom geometry;
ALTER TABLE
geonames=# update allcountries set geom = ST_SetSRID(ST_MakePoint(longitude, latitude), 4326);
UPDATE 1479415
geonames=#

geonames=#
geonames=# create index allcountries_geom_idx on allcountries using GIST(geom);
CREATE INDEX
geonames=#
geonames=# cluster allcountries using allcountries_geom_idx;
CLUSTER
geonames=#

geonames=# cluster allcountries using allcountries_geom_idx;
CLUSTER
geonames=#
geonames=# update allcountries set geohash = ST_GeoHash(geom);
UPDATE 1479415
geonames=#