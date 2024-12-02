const removeFrutas = document.getElementsByClassName("exclusaobt");
for (var i =0; i < removeFrutas.length; i++) {
    removeFrutas[i].addEventListener("click", function(event) {
        event.target.parentElement.remove()
    })
}