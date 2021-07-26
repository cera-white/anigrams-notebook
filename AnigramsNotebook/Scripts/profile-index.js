//(function ($) {

//    function ProfileIndex() {
//        var $this = this;

//        function intialize() {
//            $("#profileFile").change(function () {
//                readURL(this);
//            });
//        }

//        function readURL(input) {
//            if (input.files && input.files[0]) {
//                var reader = new FileReader();

//                reader.onload = function (e) {
//                    $('#uploading').attr('src', e.target.result);
//                }
//                reader.readAsDataURL(input.files[0]);
//            }
//        }

//        $this.init = function () {
//            intialize();
//        }
//    }

//    $(function () {
//        var self = new ProfileIndex();
//        self.init();
//    })

//})(jQuery)

window.onload = function () {
    //Check File API support
    if (window.File && window.FileList && window.FileReader) {
        $('#files').on('change', function (event) {
            var files = event.target.files; //FileList object
            var output = document.getElementById('result');
            for (var i = 0; i < files.length; i++) {
                var file = files[i];
                //Only pics
                if (file.type.match('image.*')) {
                    // continue;
                    var picReader = new FileReader();
                    picReader.addEventListener('load', function (event) {
                        var picFile = event.target;
                        var div = document.createElement('div');
                        div.setAttribute('class', 'grid-item grid-item--width2')
                        div.innerHTML = "<a data-fancybox='gallery' href='" + picFile.result + "'><img src='" + picFile.result + "' alt='preview image' /></a>";
                        var $div = $(div);
                        $grid.append($div).masonry('appended', $div);
                        $grid.masonry('reloadItems');
                        $grid.masonry('layout');
                    });
                    picReader.readAsDataURL(file);
                } else {
                    alert('You can only upload images.');
                    $(this).val('');
                }
            }
        });
    }
    else {
        console.log('Your browser does not support File API');
    }
}