
// function accepts either a ASP.Net literal or bulleted list and appropriately shows errors
function handleError(errorContainerName, errorContainerWrapperName ,bulletErrorContainerName, errorModel)
{
    let errorContainer = document.getElementsByClassName(errorContainerName);
	let bulletErrorContainer = document.getElementById(bulletErrorContainerName);
    if (bulletErrorContainer != undefined) {
    	if (typeof errorModel === 'object')
		{
			$(bulletErrorContainer).append('<li><span class="tab">' + errorModel.ErrorMessage + '</span></li>');
		}
		else if(typeof errorModel === 'string')
		{
			$(bulletErrorContainer).append('<li><span class="tab">' + errorModel + '</span></li>');
		}
        
    }else if (errorContainer != undefined) 
    {
        $(errorContainerWrapperName).show();
        if (typeof errorModel === 'object')
		{
			$(errorContainerName).append('<li><span class="tab">' + errorModel.ErrorMessage + '</span></li>');
		}else
		{
			$(errorContainerName).append('<li><span class="tab">' + errorModel + '</span></li>');
		}
        
    }
    else {
        alert("Error handling failure");
    }

}




