$(
 function () {
     var viewModel = {
         name: ko.observable("bob"),
         nameVisible:ko.observable(true),
         changeName: function () {
             this.name("steve");
         },
         
     };

     viewModel.displayName=
     ko.computed(function() {
         return "is" + this.nameVisible();
     }, viewModel);

     ko.applyBindings(viewModel);  
 }
);