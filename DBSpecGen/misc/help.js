function tabClick(iTab)
{
  	if(!iTab)iTab=0;
  	for(var i=0;i<helpTab.length;i++)
  	{
  		helpTab[i].style.backgroundColor=['transparent','menu'][(iTab==i)*1];
  		helpTab[i].style.zIndex=(iTab==i)*2;
  	}
  	
  	for(var i=0;i<tabContent.length;i++)
  	{
  		tabContent[i].style.display=['none','block'][(iTab==i)*1];
  	}
}

function SearchFor(text)
{
	parent.frames.left.document.all.searchResults.innerHTML="";
	if(!text)
	{
		alert('Enter something to search for, please.');
		return;
	}
	
	text = text.replace(/"/,"&quot;").replace(/&/,"&amp;").replace(/</,"&lt;").replace(/>/,"&gt;")
	var pages = xmlIndex.selectNodes("/root/page");  // xmlIndex is an xml island in the left hand pane... 
	var andClauses = text.split(' and ');
	var xmlResult="<div>";
	var numPagesFound=0;
	var bFound=false;
	
	for(var i=0;i<pages.length;i++)
	{
		var title = pages[i].selectSingleNode("@title").value;
		var fileName = pages[i].selectSingleNode("@name").value;
		for(var j=0;j<andClauses.length;j++)
		{
			var orClauses = andClauses[j].split(' ');
			for(var k=0;k<orClauses.length;k++)
			{
				var re = new RegExp(orClauses[k],"i");  	
				if(re.test(pages[i].text))
				{
					bFound=true;
				}
				else
				{
					bFound=false;
					break;
				}
			}
			if(!bFound)
			{
				break;
			}
		}
		
		if(bFound)
		{
			xmlResult+="<br/><span class=\"menuItem\" onmouseover=\"event.srcElement.style.color='blue'\" "
			xmlResult+="onmouseout=\"event.srcElement.style.color='black'\" "
			xmlResult+="onclick=\"parent.frames.right.navigate('"+fileName+"'); ColorPage('"+text+"',true);\">"+title+"</span>";
			numPagesFound+=1;
		}
	}
	
	if(!numPagesFound) 
	{
		xmlResult+="<span class=\"text\">No pages match your search.</span>";
	}
	xmlResult+="</div>";
	
	parent.frames.left.document.all.searchResults.innerHTML=xmlResult;
}

function ColorPage(highlightText, bWait)
{
	if(bWait)
	{
		window.setTimeout("ColorPage('"+highlightText+"', false)", 100);
	}
	else
	{
		var andClauses=highlightText.split(' and ');
		for(var j=0;j<andClauses.length;j++)
		{
			var aText=andClauses[j].split(' ');
			for(var i=0;i<aText.length;i++)
			{
				var rng=parent.frames.right.document.body.createTextRange();
				rng.collapse();
				while(rng.findText(aText[i]))
				{
					rng.execCommand('BackColor',false,'#ffff00');
					rng.collapse();
					rng.move("word");
				}
			}
		}
	}
}


function ToggleShowChildren(ParentID)
{
	var allImgs = document.getElementsByTagName("img");
	for(var i=0;i<allImgs.length;i++)
	{
		if(allImgs[i].PageID && allImgs[i].PageID==ParentID)
		{
			if(allImgs[i].src.indexOf("odir.gif")>-1)
			{
				allImgs[i].src="images/cdir.gif";
			}
			else if(allImgs[i].src.indexOf("cdir.gif")>-1)
			{
				allImgs[i].src="images/odir.gif";
			}
			else if(allImgs[i].src.indexOf("collapsed.gif")>-1)
			{
				allImgs[i].src="images/expanded.gif";
			}
			else if(allImgs[i].src.indexOf("expanded.gif")>-1)
			{
				allImgs[i].src="images/collapsed.gif";
			}
		}	
	}
	
	var allNodes = document.getElementsByTagName("span");
	for(var i=0;i<allNodes.length;i++)
	{
		if(allNodes[i].parentID && allNodes[i].parentID==ParentID)
		{
			allNodes[i].style.display=(allNodes[i].style.display=="none") ? "block" : "none";
		}
	}
}

function SynchronizeTree(PageID)
{
	CollapseAll();
	parent.left.document.all.helpTab[0].click();
	var currentPageID = PageID;
	var parentID;
	while(true)
	{
		parentID = parent.left.document.all[currentPageID].parentID;
        	if(parentID)
        	{
			ExpandParent(parentID);
			currentPageID = parentID;
        	}
        	else
        	{
			break;
        	}
	}	
}	

function ExpandParent(parentID)
{
    	var allImgs = document.getElementsByTagName("img");
	for(var i=0;i<allImgs.length;i++)
	{
		if(allImgs[i].PageID && allImgs[i].PageID==parentID)
		{
			if(allImgs[i].src.indexOf("cdir.gif")>-1)
			{
				allImgs[i].src="images/odir.gif";
			}
			else if(allImgs[i].src.indexOf("collapsed.gif")>-1)
			{
				allImgs[i].src="images/expanded.gif";
			}
		}	
	}
	
	var allNodes = document.getElementsByTagName("span");
	for(var i=0;i<allNodes.length;i++)
	{
		if(allNodes[i].parentID && allNodes[i].parentID==parentID)
		{
			allNodes[i].style.display="block";
		}
	}
}

function CollapseAll()
{
	var allNodes = document.getElementsByTagName("span");
	for(var i=0;i<allNodes.length;i++)
	{
		if(allNodes[i].parentID)
		{
			allNodes[i].style.display="none";
		}
	}
	
	var allImgs = document.getElementsByTagName("img");
	for(var i=0;i<allImgs.length;i++)
	{
		if(allImgs[i].src.indexOf("expanded.gif")>-1)
		{
			allImgs[i].src="images/collapsed.gif";
		}
		
		if(allImgs[i].src.indexOf("odir.gif")>-1)
		{
			allImgs[i].src="images/cdir.gif";
		}	
	}
}

function ExpandAll()
{
	var allNodes = document.getElementsByTagName("span");
	for(var i=0;i<allNodes.length;i++)
	{
		if(allNodes[i].parentID)
		{
			allNodes[i].style.display="block";
		}
	}
	
	var allImgs = document.getElementsByTagName("img");
	for(var i=0;i<allImgs.length;i++)
	{
		if(allImgs[i].src.indexOf("collapsed.gif")>-1)
		{
			allImgs[i].src="images/expanded.gif";
		}
		
		if(allImgs[i].src.indexOf("cdir.gif")>-1)
		{
			allImgs[i].src="images/odir.gif";
		}		
	}
}