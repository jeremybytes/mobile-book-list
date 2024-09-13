$bookListFileName = "raw_book_list.csv"
$laserBookFileName = "laser_books.csv"

$sourcePath = "C:\Development\Apps\BookList\Data\"
$destinationPath = "C:\Development\Apps\BookList\DataScripts\"

$bookListSourcePath = join-path $sourcePath $bookListFileName
$bookListDestinationPath = join-path $destinationPath $bookListFileName

Copy-Item -Path $bookListSourcePath -Destination $bookListDestinationPath

$laserBookSourcePath = join-path $sourcePath $laserBookFileName
$laserBookDestinationPath = join-path $destinationPath $laserBookFileName

Copy-Item -Path $laserBookSourcePath -Destination $laserBookDestinationPath