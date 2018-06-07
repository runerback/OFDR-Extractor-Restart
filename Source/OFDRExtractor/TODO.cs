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