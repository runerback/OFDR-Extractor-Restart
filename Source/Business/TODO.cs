using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OFDRExtractor.Business
{
	//handle folders first, extract files and move to container folder after all folders are in position.
	//get about 5 front folder name order by index, then compare with folder tree built from ProcessMonitor result.
	//calculated folder tree should same as before.
	//-------------------------------update 1-----------------------------------------------//
	//get all layer 1 to leaf branch path from prepared folder tree, then search unique nfs folder from data_win or folder later than it.
	//travel nfs folders by order, after match a whole prepared branch, move each folder to it's parent folder
	//-------------------------------update 2-----------------------------------------------//
	//use branches.txt directly because it's ordered. build branches from file system will break order.
	//so the prepared branches will be loaded from branches.txt now.
}
