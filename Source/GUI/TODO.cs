//handle folders first, extract files and move to container folder after all folders are in position.
//get about 5 front folder name order by index, then compare with folder tree built from ProcessMonitor result.
//calculated folder tree should same as before.
//-------------------------------update 1-----------------------------------------------//
//get all layer 1 to leaf branch path from prepared folder tree, then search unique nfs folder from data_win or folder later than it.
//travel nfs folders by order, after match a whole prepared branch, move each folder to it's parent folder
//-------------------------------update 2-----------------------------------------------//
//use branches.txt directly because it's ordered. build branches from file system will break order.
//so the prepared branches will be loaded from branches.txt now.
//-------------------------------update 3-----------------------------------------------//
//set related core file as EmbedResources and never output, only expose branches.txt (which is made by myself...)
//the whole plan: read all nfs lines -> build nfs root with branches -> unpack
//the first two steps will be used in Refresh operation on UI
//-------------------------------update 4-----------------------------------------------//
//Remove prepared folder/file
//change ISelectable to nullable in FolderData
//add ParentFolder to FolderData
//hide CheckBox if folder does not contain any file or folder
//Selection still doesn't work well
//-------------------------------update 5-----------------------------------------------//
//the close symbol not work in all computer, need to change
//the selected files count not update
//cannot extract file from this, but unpack directly works.
//extractor in core does not need report progress for each file unpack operation.
//unpack operation block the UI, put to task queue.
//show checkbox of root folder only when any files checked, or it will cost a long time. (resolve in later)
//-------------------------------update 6-----------------------------------------------//
//set WorkingDirectory on ExtractorInvoker (discard)
//folder shows not select all when any empty inner folder
//-------------------------------update 7-----------------------------------------------//
//there are some files cannot be unpacked, get the list then check what's the problem.
//check whether these files can be unpacked via directly call.
//-------------------------------update 8-----------------------------------------------//
//check whether all 'mission.xml' can be unpacked properly, if true,
//when file with order greater than 0 unpack failed, unpack with default order 0.