# Cactus-Assorted-CK3-Map-Tools
A few tools to assist in the map creation process, some based off of tools from CK2 such as CK2maptools.

1. Colorer

	The colorer reads definitions.csv and puts unused values on grey (or any color not in definitions.csv) provinces. The existing setting must be checked if there are already provinces on the map to prevent potential repeats. Also has a non-contiguous province detection mode, which prevents tiny coastal islands that belong to other provinces, or groups of small lakes, from each being assigned their own province color. 
	
2. Map creator

	The map creator requires at the least an image with red dots for the center of provinces. Colors are used to define the terrain type, which will be automatically set up in 00_province_terrain.txt. Provinces are automatically generated, with both shape and color, there is a degree of randomness so regenerating or moving province centers may give better results. There is a section in the config for landed titles, they are currently user defined and you paint the titles onto the map based on the colors in the respective files. What matters the most is if the center of the province is inside it's landed title, the whole province doesn't have to be but it recommended to prevent user mistakes. I find it easier progressing from the bottom-up, creating counties based on provinces, then duchies based on counties, etc. 
