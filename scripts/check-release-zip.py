"""Checks that a release zip holds exactly the files of an unpacked release.

usage: check-release-zip.py <zip> <directory>

The release is published as a tarball and a zip with the same contents. The
tarball is what gets unpacked and tested, so this is what vouches for the zip.
"""

import os
import sys
import zipfile


def main(archive, directory):
    with zipfile.ZipFile(archive) as z:
        broken = z.testzip()
        if broken is not None:
            sys.exit("%s: %s is corrupt" % (archive, broken))
        in_zip = {name for name in z.namelist() if not name.endswith("/")}

    on_disk = set()
    for folder, _, files in os.walk(directory):
        for name in files:
            relative = os.path.relpath(os.path.join(folder, name), directory)
            on_disk.add(relative.replace(os.sep, "/"))

    if in_zip != on_disk:
        differing = sorted(in_zip ^ on_disk)
        sys.exit("%s differs from %s in %d files, for example: %s"
                 % (archive, directory, len(differing), ", ".join(differing[:10])))


if len(sys.argv) != 3:
    sys.exit(__doc__)
main(sys.argv[1], sys.argv[2])
