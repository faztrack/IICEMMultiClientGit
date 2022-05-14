// JScript File

function checkTextAreaMaxLength(textBox,e, length)
{
    
        var mLen = textBox["MaxLength"];
        if(null==mLen)
            mLen=length;
        
        var maxLength = parseInt(mLen);
        if(!checkSpecialKeys(e))
        {
         if(textBox.value.length > maxLength-1)
         {
            if(window.event)//IE
              e.returnValue = false;
            else//Firefox
                e.preventDefault();
         }
    }   
}

function checkTextAreaMaxLengthWithDisplay(textBox,e, length,display)
{
    
       var len = textBox.value.length;
        var mLen = textBox["MaxLength"];
        if(null==mLen)
            mLen=length;
        
        var maxLength = parseInt(mLen);
        //alert('masud');
        display.value = '(Length: '+len+' of '+ length+')';
        display.style.color = 'black';
        if(!checkSpecialKeys(e))
        {
         if(textBox.value.length > maxLength-1)
         {
            if(window.event)//IE
            {
              e.returnValue = false;
              display.style.color = 'red';
              }
            else//Firefox
            {
                e.preventDefault();
                
                }
         }
         
    }   
}


function checkSpecialKeys(e)
{
    if(e.keyCode !=8 && e.keyCode!=46 && e.keyCode!=37 && e.keyCode!=38 && e.keyCode!=39 && e.keyCode!=40)
        return false;
    else
        return true;
}
function ShowProgress() {
    parent.document.getElementById("LoadingProgress").style.display = 'block';
}

function HideProgress() {
    parent.document.getElementById("LoadingProgress").style.display = 'none';
}

function ShowProgressCircle(id) {
    parent.document.getElementById(id).style.display = 'block';
}

function HideProgressCircle(id) {
    parent.document.getElementById(id).style.display = 'none';
}