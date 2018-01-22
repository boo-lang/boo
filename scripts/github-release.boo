""" Simple script to upload a new release asset to GitHub.

    Authentication is done using oAuth2 tokens. The token to use should 
    be available as an environment variable named GITHUB_TOKEN.

    usage: booi github-release.boo -- <release> <path/to/build.zip>

    Note that It will remove any prior assets with the same name before
    uploading the new one.

"""

from System import Environment, Uri
from System.IO import Path, File
from System.Net import WebClient, WebException, HttpWebResponse
from System.Collections.Generic import List
import System.Web.Script.Serialization from System.Web.Extensions


class Release:
""" Models the Json responses from GitHub http://developer.github.com/v3/repos/releases/
    Using dynamic types is buggy under some Mono versions, so this is the
    only solution to make it work.
"""
    class Asset:
        property url as string
        property id as int
        property name as string
        property label as string
        property state as string
        property content_type as string
        property size as int
        property download_count as int
        property created_at as string
        property updated_at as string

    property assets as List[of Asset]
    property url as string
    property html_url as string
    property assets_url as string
    property upload_url as string
    property id as int
    property tag_name as string
    property target_commitish as string
    property name as string
    property body as string
    property draft as bool
    property prerelease as bool
    property created_at as string
    property published_at as string


def client():
""" WebClient doesn't keep the headers between requests, so we instantiate
    a new one every time we want to use it.
"""
    wc = WebClient()
    wc.Headers['Authorization'] = 'token ' + Environment.GetEnvironmentVariable('GITHUB_TOKEN')
    # The API requires this custom Accept header while in beta
    wc.Headers.Add('Accept', 'application/vnd.github.manifold-preview')
    wc.Headers.Add('user-agent', 'boo-lang')
    return wc

if len(argv) < 2:
    print 'Usage: github-release <release> <path/to/build.zip>'
    return

if not Environment.GetEnvironmentVariable('GITHUB_TOKEN'):
    raise 'GITHUB_TOKEN environment variable not set'

MAX_UPLOAD_RETRIES = 5
USERNAME = 'boo-lang'
REPO = 'boo'
API_URL = "https://api.github.com/repos/$USERNAME/$REPO/releases"
RELEASE_NAME = argv[0]
ASSET_FILE = argv[1]
ASSET_NAME = Path.GetFileName(ASSET_FILE)

json = JavaScriptSerializer()

print "Looking for release $RELEASE_NAME"
try:
    result = client().DownloadString(API_URL)
except wx as WebException:
    var e = wx.Response cast HttpWebResponse
    using sr = System.IO.StreamReader(e.GetResponseStream()):
        print sr.ReadToEnd()
    raise
releases = json.Deserialize[of List[of Release]](result)
release = releases.Find({rel | rel.tag_name == RELEASE_NAME})

if not release:
    raise "Release $RELEASE_NAME not found"

for asset in release.assets:
    if asset.name == ASSET_NAME:
        print "Removing asset $(asset.id)"
        client().UploadData(asset.url as string, "DELETE", array(byte, 0))
        break

upload_url = (release.upload_url as string).Replace('{?name,label}', "?name=$ASSET_NAME")
print "Uploading $ASSET_FILE to $upload_url"
retries = 0
:retry
try: 
    using uploadClient = client():
        uploadClient.Headers.Add("Content-Type", "application/zip")
        using fileStream = File.OpenRead(ASSET_FILE), requestStream = uploadClient.OpenWrite(Uri(upload_url), "POST"):
            fileStream.CopyTo(requestStream)
except ex as WebException:
    var e2 = ex.Response cast HttpWebResponse
    using sr = System.IO.StreamReader(e2.GetResponseStream()):
        print sr.ReadToEnd()
    retries += 1
    if retries >= MAX_UPLOAD_RETRIES:
        print "Too many retries, giving up..."
        raise ex 
    print "Upload failed ($(ex.Message)), retrying..."
    goto retry

print 'Upload successful.'