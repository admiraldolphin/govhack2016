#Data munger Readme
Ok so in here is a collection of different little scripts and apps we wrote to munge data into a format that the game server could then present to the game.

## NewsCorpScraper
A crude scraper app written in Swift for the News Corp API.
Relies of the API keys listed in the govhack documentation.
The News Corp API often just doesn't return data for whatever reason...

This code uses [SwiftyJSON](https://github.com/SwiftyJSON/SwiftyJSON) which is under the MIT License.

## Fake Headlines
A quick machine learning markov chain generated written in Python. It reads from the `corpus.txt` file included which is a list of headlines generated from the ABC news api.

This code uses [Markovify](https://github.com/jsvine/markovify) which is under the MIT license.