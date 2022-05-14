<%@ Page Title="" Language="C#" MasterPageFile="~/Main.master" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="Test" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
      <link href="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" rel="stylesheet">
<script src="https://code.jquery.com/jquery-3.5.1.min.js"></script>
<script src="https://stackpath.bootstrapcdn.com/bootstrap/3.4.1/js/bootstrap.min.js"></script>

<!-- include summernote css/js -->
<link href="https://cdn.jsdelivr.net/npm/summernote@0.8.18/dist/summernote.min.css" rel="stylesheet"/>
<script src="https://cdn.jsdelivr.net/npm/summernote@0.8.18/dist/summernote.min.js"></script>

    <script>
        $(document).ready(function () {
             $("#summernote").summernote({               
                
                height: 300
            });
        });
    </script>
    <div id="summernote"></div>

    <button id="btnGetEmployee"> Click</button>








     <script type="text/javascript">
         $(document).ready(function () {



     $('#btnGetEmployee').click(function () {


                       var obj = {};
  var markupStr = $('#summernote').summernote('code');
    obj.Message = markupStr;
    obj.Name = "Test";
  


    var jsonData = JSON.stringify(obj);

    $.ajax({
        url: 'EmailHandler.ashx',
        type: 'POST',
        data: jsonData,
        success: function (data) {
         
            console.log(data);
            alert("Success :" + data);
        
        },
        error: function (errorText) {
            alert("Wwoops something went wrong !");
        }
    });

    e.preventDefault();
      
  });

  });
     </script>
       
    
</asp:Content>

