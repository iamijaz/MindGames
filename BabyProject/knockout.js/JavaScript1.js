$(
 function () {
     var viewModel = {
         name: "bob",
         changeName: function () {
             this.name="steve";
         }
     };     
     ko.applyBindings(viewModel);  
 }
);