ImagePerf
=========

A simple tool created to resize images loaded on a server to a smaller poster area so that then can load alot faster
The tool also takes into consideration mistakes that may be made during procesing (resizing an already small image size)
so a post processing mechanism allows for revalidation of file-sizes if the src image is less than the compress image the 
compressed image is then replaced therefore maintaining a smaller file offset