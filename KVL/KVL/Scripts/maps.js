
function chamaMapa() {
    //aqui vamos definir as coordenadas de latitude e longitude no qual //sera exibido o nosso mapa
    var latlng = new google.maps.LatLng(-3.7957374, -38.492154);
    //DEFINE A LOCALIZAÇÃO EXATA DO MAPA //aqui vamos configurar o mapa, como o zoom, o centro do mapa, etc
    var myOptions = {
        zoom: 15,
        //utilizaremos o zoom 15
        center: latlng,//aqui a nossa variavel constando latitude e //longitude ja declarada acima
        mapTypeId: google.maps.MapTypeId.ROADMAP
    };
    //criando o mapa dentro da div com o id="map_canvas" que ja criamos
    map = new google.maps.Map(document.getElementById("map_canvas"), myOptions);
    //DEFINE AS COORDENADAS do ponto exato - CENTRALIZAÇÃO DO MAPA
    map.setCenter(new google.maps.LatLng(-3.7957374, -38.492154));
}

function chamaMarcacaoComTextoInfo(logradoro, campo) {
    //colocando o endereco no padrao correto exigido pelo googlemaps
    //var endereco = logradoro + ", " + numero + ", " + bairro + " - " + cidade;
    var endereco = logradoro + " - Fortaleza";
    //mudando o zoom do mapa, de acordo com o que necessite, neste caso: 17
    //map.setZoom(17);
    //Buscando lat e log por endereco (no formato: nome rua, numero, bairro - cidade) que é o padrão.
    var geocoder = new google.maps.Geocoder(); geocoder.geocode({ 'address': endereco },
        function (results, status) {
            //se o retorno de status for ok
            if (status = google.maps.GeocoderStatus.OK) {
                //pega o retorno da variavel result, que sao a latitude e longitude do endereco
                var latlng = results[0].geometry.location;
                //faz marcacao no mapa na posição da latitude e longitude adiquirida
                var marker = new google.maps.Marker({ position: latlng, map: map }); map.setCenter(latlng);
                //leva o mapa para a posição no mapa onde foi realizado a marcacao
            } //AQUI A VARIAVEL QUE CRIAMOS PARA COLOCAR O TEXTO INFORMATIVO NA MARCAÇÃO

            var infowindow = new google.maps.InfoWindow(), marker;
            //AQUI O EVENTO GOOGLEMAPS QUE COLOCA O TEXTO INFORMATIVO NA MARCACAO
            google.maps.event.addListener(marker, 'click', (function (marker, i) {
                return function () {
                    //COLOCANDO O TEXTO INFORMATIVO NO EVENTO GOOGLEMAPS
                    infowindow.setContent("Campo : " + campo);
                    //A VARIAVEL QUE LIGA O TEXTO INFORMATIVO A VARIAVEL DE MARCACAO E REALIZA A MARCACAO NO MAPA.
                    infowindow.open(map, marker);
                }
            })(marker))
        });
}