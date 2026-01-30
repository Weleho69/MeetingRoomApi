### 1. Mitä tekoäly teki hyvin?
Mielestäni neuvot suht selkeitä ja vaihtoehtoja useampi, joista ei hirveästi jäänyt epäselvyyksiä mitä ja miksi. Ns. "tyhmät" kysymykset voitiin kysyä ja saada suora vastaus.
Rakenteet mielestäni suht selkeitä. Myös ns. "boilerplate"-koodia saatiin nopeasti ja helposti.

### 2. Mitä tekoäly teki huonosti?

Tekoäly ei monesti ottanut huomioon relaatioita hirveän hyvin. Päivitys funktiot olivat virheellisiä, siinä yritettiin luoda uutta riviä kantaan eikä päivittää valittua riviä.
Poisto funktioissa ei monesti otettu relaatiota huomioon esim. foreign keyt

### 3. Mitkä olivat tärkeimmät parannukset, jotka tein tekoälyn tuottamaan koodiin ja miksi?
Korjasin em. relaatio ongelmat, päivitys pyyntöjen ongelmat. Lisäsin uusia obejkteja, joilla saadaan esim. asiakkaasta enemmän tietoa.
Lisätty pyyntöjen rollback ominaisuus käyttäen EF-coren transaction, joilla voidaan lisätä vikasietoa ja varmistaa tiedon eheys pyyntöjä tehdessä.
Lisäsin hieman selkeyttä pyyntöjen vastauksiin ihmislukuisuuden ja tietojen vuoksi.