05 Feb 2021
============

Objective:
Cluster geographically placed points
For eg, we have 30K sites, cluster them into around 50 clustered nodes and display only 50 points on Google Maps

Cluster Algo: GeoHash
	There are other possibilities as well. We should try some to see which one is best suited for us. K-Means seems one such Algo found frequently on internet

Database
	PostgreSQL + PostGIS
	Other popular alternative is ElasticSearch which supports geohash based clustering natively
---------------------------------

WebAPI
	Expose an HTTP endpoint for UI consumption, and access database
	Use DotNet Core 3.1, and NpgSql Nuget
---------------------------------

UI
	Create simplest possible UI
	Calls WebAPI to get clustered points to display on a Google Map 
========================

08 Feb 2021
-----------

On Performance
	1. The allcountries table has around 1.4+ million points
	2. group by ST_GeoHash(geom, {precision})
		a. no index, precision = 1, all world => 12 sec
		b. with clustered index, 7 to 8 sec
		c. on a subset of 42K points => 440 ms, on a subset of 17K points => 350 ms
	3. substring(geohash, 1, {precision}) as cluster_hash, group by cluster_hash
		a. no index on string column, precision = 1, all world => xx sec
			seemed like it gave incorrect/different results
		b. indexed, precision = 1, all world => 17 sec, 23 sec, 7 sec, 8 sec ??
		c. On smaller subsets, seems either equivalent or slower than using ST_GeoHash

