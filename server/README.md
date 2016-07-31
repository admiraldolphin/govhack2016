# Beat the News server 

## Installation

Use `go get`:

    go get github.com/admiraldolphin/govhack2016/server

This should produce a `server` binary inside `$GOPATH/bin/`.

## Running

The server expects the base file paths of different data sets to be passed via flag. Here's an example where the various data sets happen to be in `~/govhack2016/`:

    ./server -abc_base ~/govhack2016/abc \
	         -npg_base ~/govhack2016/npg \
			 -linc_base ~/govhack2016/linc \
			 -newscorp_base ~/govhack2016/newscorp \
			 -csiro_base ~/govhack2016/csiro \
			 -abclocal_base ~/govhack2016/abclocal

The server logs when each data set is loaded, and should also log for every request. The data format for each data set isn't documented especially, and is inconsistent. (Sorry. It did make sense at the time.) It should be easy to figure out from the source code (e.g. the ABC loader looks for a Summary\_NNNNNNN.json in the NNNNNNN directory).

For other flags look at the output of `server -help`.