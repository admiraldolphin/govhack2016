//
//  AppDelegate.swift
//  NewsCorpScraper
//
//  Created by Tim Nugent on 30/07/2016.
//  Copyright © 2016 Tim Nugent. All rights reserved.
//

import Cocoa

@NSApplicationMain
class AppDelegate: NSObject, NSApplicationDelegate {

    @IBOutlet weak var window: NSWindow!
    
    var numberOfLumps = 0
    
    // scraped from the website because their collections API is a piece of hezmana
    let urlIDs = ["2f1decd3f3b94955a147daaa0b2f88d2",
                  "18f99dc0e363fc4f6b54281c5f128f7b",
                  "c0dfa547fd1279266312c101129199f8",
                  "57de8c5161c06b1b39b1a9a4ecac46ec",
                  "62686e97969953036710141fe8670fed",
                  "d6f1f40ff0917569a897e5bc775d429c",
                  "f8ff9c26b9ffd0ba73f63841a9461567",
                  "9a2a3928e99d81fb0082f188d07aabca",
                  "f1a2111b94affdc7c62f1c893747b151",
                  "21ddd8888e3d518af17a1b95b038bbf5",
                  "b3961e4ec7acc02ead27d412639b37c1",
                  "b1b3560b9a59123e4b6f89435b1549b8",
                  "a77a925839fdc33cc9c0433fc65d63ea",
                  "9467afdd74c64058d5a1ef0fc148ed95",
                  "fecd5972a751441f80de6c1c5ec869c1",
                  "c101f956d15464d2f0dc3007a7297184",
                  "f18db0d766cc71915a4c2de6eccb68aa",
                  "340db2ea551d3b3e3de96f007b5d0ee2",
                  "c8b56098db0f57c6334f46649a0c81bb",
                  "b5eae36d14dec02ef08f71957f0534bb",
                  "27acd7306f3621c933be2c2f8aa74caa",
                  "18ddd893d3cce4307166653b32a1372a",
                  "53cd89004d438b60fa59ec910896759f",
                  "cfccc3141305228fcf071a4bb318ae00",
                  "b4c2aaff47f0b63dd0f1e5b2e9520812",
                  "846ecac243104997b234d2da29c5c2d0",
                  "8686dfbea67c22c66ba2ac1685237960",
                  "d288cfcdc97afe2cf9e561dcd93fb3de",
                  "9173052cb915b375162fe51cbfa766b0",
                  "2fb8589bd0d1a23c5745fc70f0101ea2",
                  "2c6dcf0e55a9fcf3cecc94721cd41436",
                  "c5dfc8d368719151bfa273147cbca770",
                  "f67616d1c281139c2abd51a16fe33917",
                  "8825a817e801eb3de7aee54a90dd6022",
                  "9903dbcd559a2cc9bac6fc70849f858a",
                  "143b3166b139b923b5f2af8cdbeff59b",
                  "f82f21596ad29368692c3cd3aa8b6c8b",
                  "fb4ab8b10f967e5d07c49223782957ef",
                  "42aea1b6cbe26267010b8e4f017f7b6c",
                  "e940d082871b9514d89d4ca3094e575a",
                  "2ed32e035d4a37e0fbc0c19813992c42",
                  "32a1cff2e99200a481039a7f0b8c5bb1",
                  "537ab49a98e0aeed6f2d5fc72664622d",
                  "56816c8830813ecf6b9521d585617c54",
                  "c6d51325c8d4b0043d40bfe5cd090d38",
                  "8d2c23442ff1ded2543be840f7363d8c",
                  "3cfa4a176ee7a20ed6616ca489f4a760",
                  "c27e2cbe097c4f410f11e7605f475e46",
                  "ebdbecaf4a41633b39ae8faf1d8d367c",
                  "b2962edfbab954901775ca2e11fc5eae",
                  "e1b35ffdbcf3d34853412d5fda9d8459",
                  "fc9a5860d56be385880f1408d260c723",
                  "20893b33630252e3302075235a15ee03"]
    
    var lumpyThing : [String:NSDictionary] = [:]

    func applicationDidFinishLaunching(aNotification: NSNotification) {
        // Insert code here to initialize your application
    }

    func applicationWillTerminate(aNotification: NSNotification) {
        // Insert code here to tear down your application
    }
    
    // force a dump to json regardless because the news corp API breaks all the frelling time
    @IBAction func dumpLumps(sender: AnyObject) {
        let jsonData = try! NSJSONSerialization.dataWithJSONObject(self.lumpyThing, options: NSJSONWritingOptions.PrettyPrinted)
        jsonData.writeToFile("/Path/To/Save/Folder/newscorp.json", atomically: true)
    }
    func LumpAdded(lump: String)
    {
        self.numberOfLumps += 1
        
        if self.numberOfLumps == self.urlIDs.count
        {
            if NSJSONSerialization.isValidJSONObject(self.lumpyThing)
            {
                print("its good")
                let jsonData = try! NSJSONSerialization.dataWithJSONObject(self.lumpyThing, options: NSJSONWritingOptions.PrettyPrinted)
                jsonData.writeToFile("/Path/To/Save/Folder/newscorp.json", atomically: true)
                print("DONE")
            }
            else
            {
                print("Nope")
            }
        }
        else
        {
            print("It isn't time yet \(self.numberOfLumps)/\(self.urlIDs.count): \(lump)")
        }
    }
    
    @IBAction func scrape(sender: AnyObject) {
        let configuration = NSURLSessionConfiguration.defaultSessionConfiguration()
        let session = NSURLSession(configuration: configuration, delegate: nil, delegateQueue: nil)
        
        for id in urlIDs
        {
            guard let url = NSURL(string: "http://cdn.newsapi.com.au/content/v2/\(id)?api_key=dvae65bt88b6n8gpttv4ervt") else
            {
                continue
            }
            session.downloadTaskWithURL(url, completionHandler: {(theUrl, theUrlResponse, theError) in
                if theError != nil
                {
                    print("FAILED TO GRAB \(url)")
                }
                else if let theFile = theUrl
                {
                    //downloaded it
                    // parse it as json
                    guard let theData = NSData(contentsOfURL: theFile) else
                    {
                        return
                    }
                    let json = JSON(data: theData)
                    guard let imageID = json["thumbnailImage"]["id"]["value"].string, storyTitle = json["title"].string else
                    {
                        return
                    }
                    guard let imageURL = NSURL(string: "http://cdn.newsapi.com.au/image/v1/\(imageID)?width=10000&api_key=mvpwr96kt9hmx7mkhanm6auv") else
                    {
                        return
                    }
                    session.downloadTaskWithURL(imageURL, completionHandler: { (imageLocationURL, imageResponse, imageError) in
                        if imageError != nil
                        {
                            print("ARGH ARGH ARGH \(imageURL)")
                        }
                        else if let theImageUrl = imageLocationURL
                        {
                            guard let imageData = NSData(contentsOfURL: theImageUrl) else
                            {
                                return
                            }
                            guard let thumbnail = NSImage(data: imageData) else
                            {
                                return
                            }
                            
                            var keywords : [String] = []
                            for keyword in json["keywords"]
                            {
                                if let theKeyword = keyword.1.string
                                {
                                    keywords.append(theKeyword)
                                }
                            }
                            
                            let image = thumbnail.TIFFRepresentation
                            let bitmapRep = NSBitmapImageRep(data: image!)
                            let imageProperties = [NSImageCompressionFactor:0.7]
                            let toSave = bitmapRep?.representationUsingType(NSBitmapImageFileType.NSJPEGFileType, properties: imageProperties)
                            
                            toSave?.writeToFile("/Path/To/Save/Folder/\(imageID).jpg", atomically: true)
                            
                            let lumpDict = ["image":"\(imageID).jpg","keywords":keywords]
                            self.lumpyThing[storyTitle] = lumpDict
                            self.LumpAdded(storyTitle)
                        }
                    }).resume()
                }
            }).resume()
        }
    }
}

