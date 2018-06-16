
function toggleFullScreen() {
    if (!document.fullscreenElement &&    // alternative standard method
        !document.mozFullScreenElement && !document.webkitFullscreenElement && !document.msFullscreenElement) {  // current working methods
        if (document.documentElement.requestFullscreen) {
            document.documentElement.requestFullscreen();
        } else if (document.documentElement.msRequestFullscreen) {
            document.documentElement.msRequestFullscreen();
        } else if (document.documentElement.mozRequestFullScreen) {
            document.documentElement.mozRequestFullScreen();
        } else if (document.documentElement.webkitRequestFullscreen) {
            document.documentElement.webkitRequestFullscreen(Element.ALLOW_KEYBOARD_INPUT);
        }
    } else {
        if (document.exitFullscreen) {
            document.exitFullscreen();
        } else if (document.msExitFullscreen) {
            document.msExitFullscreen();
        } else if (document.mozCancelFullScreen) {
            document.mozCancelFullScreen();
        } else if (document.webkitExitFullscreen) {
            document.webkitExitFullscreen();
        }
    }
}

//function doAction(layoutIndex, itemIndex) {
//    $.post("/api/layouts/" + layoutIndex + "/do/" + itemIndex,
//        function (data) {
//            console.log("All good running doAction(" + layoutIndex + "," + itemIndex + ")");
//        })
//        .fail(function () {
//            console.log("Error running doAction(" + layoutIndex + "," + itemIndex + ")");
//        });
//}




//function loadLayoutList() {
//    $.getJSON("/api/layouts",
//        function (data) {
//            var layoutList = $('#layoutList');
//            var currentLayoutId;
//            $.each(data, function (index, layout) {

//                if (isCurrentLayout(layout.layoutId)) {
//                    currentLayoutId = layout.layoutId;
//                }
//                const title = layout.title;
//                var id = layout.layoutId;
//                const listItem = $(`<li class="nav-item"><a class="nav-link" href="#"><span>${title}</span></a></li>`);
//                listItem.find("a").click(function (event) {
//                    location.hash = id;
//                    populateLayout(id);
//                    event.preventDefault();
//                    event.stopPropagation();
//                });
//                layoutList.append(listItem);
//            });
//            if (typeof currentLayoutId != 'undefined') {
//                //populateLayout(currentLayoutId);
//            }
//            else {
//                const firstLayout = data[0];
//                //populateLayout(firstLayout.layoutId);
//            }
//        })
//        .fail(function () {
//            console.log("Error running loadLayoutList()");
//        });
//}


function isCurrentLayout(layoutId) {
    if ("#" + encodeURI(layoutId) === location.hash) {
        return true;
    }
    return false;
}