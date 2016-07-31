#!/usr/bin/python
import json
import os
with open('ARCHDIGITISED.json') as data_file:    
    data = json.load(data_file)
d = {}
i = []
for key, photo in data.iteritems():
	if 'SUBJECT' in photo:
		path, filename = os.path.split(photo['THUMBNAIL_URL'])
		if os.path.isfile('linc/' + filename + '.jpg'):
			d[filename + '.jpg'] = {'title' : photo['TITLE'], 'subject' : photo['SUBJECT']}
			i.append(photo['THUMBNAIL_URL'])

with open('ARCHDIGITISEDclean.json', 'w') as outfile:
    json.dump(d, outfile, indent=4)		

with open('ARCHDIGITISED.wget', 'w') as outfile:
    outfile.writelines( "%s\n" % item for item in i )