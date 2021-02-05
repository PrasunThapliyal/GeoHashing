05  Feb 2021
============

Objective:
Cluster geographically placed points
For eg, we have 30K sites, cluster them into around 50 clustered nodes and display only 50 points on Google Maps

Cluster Algo: GeoHash
	There are other possibilities as well. We should try some to see which one is best suited for us. K-Means seems one such Algo found frequently on internet

Database: PostgreSQL + PostGIS
	Other popular alternative is ElasticSearch which supports geohash based clustering natively

