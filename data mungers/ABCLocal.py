#!/usr/bin/python
import json
import os
with open('Localphotostories2009-2014-JSON.json') as data_file:    
    data = json.load(data_file)
d = {}
i = []
for article in data:
	if article['Primary image'] != '':
		path, filename = os.path.split(article['Primary image'])
		d[filename] = {'title' : article['Title'], 'subject' : article['Subjects']}
		i.append(article['Primary image'])			

with open('Localphotostories2009-2014-JSONclean.json', 'w') as outfile:
    json.dump(d, outfile, indent=4)		

with open('Localphotostories2009-2014-JSON.wget', 'w') as outfile:
    outfile.writelines( "%s\n" % item for item in i )