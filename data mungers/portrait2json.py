#!/usr/bin/python
from bs4 import BeautifulSoup
import json
import os

soup = BeautifulSoup(open('portraitau-20160705.xml'))
d = {}
i = []
for portrait in soup.portraits.find_all('portrait'):
	images = portrait.images.find_all('image')
	if len(images) == 1:
		path, filename = os.path.split(images[0].fileurl.string)
		d[filename] = portrait.title.string
		i.append(images[0].fileurl.string)
		
with open('portraitau-20160705.json', 'w') as outfile:
    json.dump(d, outfile, indent=4)

with open('portraitau-20160705.wget', 'w') as outfile:
    outfile.writelines( "%s\n" % item for item in i )