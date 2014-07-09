$(
 function () {
     var viewModel = {
         name: ko.observable("bob"),
         nameVisible:ko.observable(true),
         changeName: function () {
             this.name("steve");
         }
     };     
     ko.applyBindings(viewModel);  
 }
);