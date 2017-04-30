The problem:
  We have .resx files in our solution and developers are expanding and changing them during coding.
  We also have .resx files which are on production website and which are edited by proffessional translators.
  This tool merges changes made on those sides.

How to use it?
- Copy resx folder from production to your disk
- Hit first button to set Folder 1 to that folder with resx-es from production
- Hit second button bellow to set Folder 2 to your folder with resx-ex linked with SVN	    
(Note: vice versa for folders 1 and 2 is allowed)
- Hit third button bellow to set backup folder
- Hit Synchronize
- Check log below
 if 'HasErrors:true' or 'Has conflicts:true':
  o find them in log, 
  o restore from last backup using button below 
  o fix errored and conflicted resx items manualy
  o hit Synchronize again
  o if no conflicts nor errors you can upload files to production and commit changes to SVN  

How should I add new resx item?!
 Just add it as you normaly would. Timestamps will be added on next sync automatically.

How should I edit resx item?!
 If you want to edit existing resx item directly just edit it normaly and delete timestamps from comment.	

How should I delete resx item?!
 Just delete it as you normaly would. Action will be propagated to other side on next sync automatically.		
		
How it works?
 All resx written through Admin.UI are written with Last sync.date and Last modif.date info in comment.
 Using that info this tool is able to merge items edited in under different instances of Admin.UI application and even 
 developer is able to edit files directly.
 Merge is done by rules from table below.
		
Resolution matrix -analyse row by row, letters represent dates and are related among eachothers only among one row
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////|
|                                       |                                            |                                            | 
|      resx left item                   |       resx right item                      |                                            | 
|------------|------------|-------------|-----------------|-------------|------------|--------------------------------------------|
|    Text    | Last sync. |  Last modif.|      Text       | Last sync.  |Last modif. |Resolution                                  | 
|            |    date    |     date    |                 |    date     |  date      |                                            |
|------------|------------|-------------|-----------------|-------------|------------|--------------------------------------------|
|     A      |            |             |       A         |             |            | add dates                                  |
|     A      |            |             |                 |             |            | insert to right,add dates,rebuild required |
|            |            |             |       B         |             |            | insert to left,add dates,rebuild required  |
|     A      |     S      |     S       |       B         |             |            | copy to left, update dates                 |
|     A      |            |             |       B         |     S       |     S      | copy to right, update dates                |
|     A      |     S      |     M       |       B         |             |            | conflict                                   |
|     A      |            |             |       B         |     S       |     M      | conflict                                   |
|     any    |     S      |     any     |       any       |     K       |    any     | error                                      |
|     A      |     S      |     S       |       A         |     S       |     S      | do nothing                                 | 
|     A      |     S      |     S       |       B         |     S       |     S      | error                                      |
|     A      |     S      |     M       |       B         |     S       |     S      | copy to right, update dates                |
|     A      |     S      |     S       |       B         |     S       |     M      | copy to left, update dates                 |
|     A      |     S      |     M       |       B         |     S       |     Q      | conflict                                   |
|     A      |     S      |     any     |              n  o t   f o u n d            | delete left                                |
|           n  o t   f o u n d          |       A         |     S       |     any    | delete right                               |
|     A      |            |             |              n  o t   f o u n d            | copy to right, update dates                |
|           n  o t   f o u n d          |       A         |             |            | copy to left, update dates                 |
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////|