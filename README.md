Console application for generating file signatures.

Task:
> The file signature is calculated as follows: split the file into blocks of the specified size and calculate the sha256 hash for each block. Then print the block number and hash to the console.
>
> App should be multithreaded and should be able to process large files (more than RAM size).
>
> The file name and block size are specified by command line parameters.
>
> Limitations: threads should be used (no ThreadPool, BackgroudWorker or TPL).
