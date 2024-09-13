$sourcePath = "C:\Development\Apps\BookList\DataScripts\"
$destinationPath = "C:\Development\Apps\BookList\Code\BookListLibrary\"
$appDestinationPath = "C:\Development\Apps\BookList\Code\MobileBookViewer\Resources\Raw\"

$bookListFileName = "book_list.json"

$bookListSourcePath = join-path $sourcePath $bookListFileName
$bookListDestinationPath = join-path $destinationPath $bookListFileName
$bookListAppDestinationPath = join-path $appDestinationPath $bookListFileName

Copy-Item -Path $bookListSourcePath -Destination $bookListDestinationPath
Copy-Item -Path $bookListSourcePath -Destination $bookListAppDestinationPath

$laserBookFileName = "laser_books.json"
$laserBookSourcePath = join-path $sourcePath $laserBookFileName
$laserBookAppDestinationPath = join-path $appDestinationPath $laserBookFileName

Copy-Item -Path $laserBookSourcePath -Destination $laserBookAppDestinationPath