from pathlib import Path
import re, math, json, hashlib
from reportlab.pdfgen import canvas
from reportlab.lib.colors import HexColor, white
from reportlab.pdfbase import pdfmetrics
from reportlab.pdfbase.ttfonts import TTFont
from reportlab.platypus import Paragraph
from reportlab.lib.styles import ParagraphStyle
from xml.sax.saxutils import escape
import pypdfium2 as pdfium

ROOT=Path('C:/UnityProjects/SeriousGame_AR')
OUT=ROOT/'output/pdf'; OUT.mkdir(parents=True,exist_ok=True)
TMP=ROOT/'tmp/pdfs'
for name,file in [('Sans','arial.ttf'),('Bold','arialbd.ttf'),('Mono','consola.ttf')]:
    pdfmetrics.registerFont(TTFont(name,str(Path('C:/Windows/Fonts')/file)))
INK=HexColor('#172633'); BLUE=HexColor('#234f70'); PALE=HexColor('#edf3f7'); GRAY=HexColor('#556575')

# Extract actual declarations; no inferred methods, fields or compiler-generated accessors.
files=list((ROOT/'Assets/_Project/Scripts').rglob('*.cs'))+list((ROOT/'Assets/_Project/ScriptableObjects').rglob('*.cs'))
classes={}; sources={}
for file in files:
    text=file.read_bytes().decode('utf-8',errors='replace')
    matches=list(re.finditer(r'public class (\w+)(?:\s*:\s*(\w+))?',text))
    for idx,m in enumerate(matches):
        name,base=m.groups(); body=text[m.end():matches[idx+1].start() if idx+1<len(matches) else len(text)]
        attrs=[]; events=[]; ops=[]
        for raw in body.splitlines():
            line=raw.split('//')[0].strip()
            p=re.match(r'\[field: SerializeField\]\s+public\s+(\w+)\s+(\w+)\s*\{ get; private set; \}',line)
            if p:
                typ,n=p.groups(); mult=' [0..1]' if typ in ('BuildingModel','string') else ''; attrs.append(f'+ {n}: {typ}{mult} {{C#: get public; set private}}'); continue
            line=re.sub(r'^\[SerializeField\]\s*','',line)
            f=re.match(r'(public|private)\s+(event\s+)?([\w<>,\[\]]+)\s+(\w+)\s*(?:=\s*([^;]+))?;',line)
            if f:
                vis,event,typ,n,default=f.groups(); s=f'{"+" if vis=="public" else "-"} {n}: {typ}'
                if typ in ('BuildingModel','BuildingData','GridManager','InputManager','BuildingUI','BuildingPreview','Camera','MeshCollider','GameObject','Material','Button'): s+=' [0..1]'
                if typ=='BuildingShapeUnit[]': s=f'- {n}: BuildingShapeUnit [0..*] {{ordered, nonunique}}'
                if typ=='BuildingGridCell[,]': s=f'- {n}: BuildingGridCell [0..*] {{C#: 2D-Array}}'
                if typ=='List<Renderer>': s=f'- {n}: Renderer [0..*] {{ordered, nonunique}}'
                if default: s+=' = '+default.rstrip('f')
                (events if event else attrs).append(s); continue
            op=re.match(r'(?:(public|private)\s+)?(void|bool|Vector[23]|BuildingModel|List<Vector3>|\(int x, int z\))\s+(\w+)\((.*)\)',line)
            if op:
                vis,ret,n,params=op.groups(); arguments=[]
                for param in params.split(',') if params else []:
                    typ,pname=param.strip().rsplit(' ',1); arguments.append(f'{pname}: {typ}')
                suffix='' if ret=='void' else ': '+('GridCoordinates' if ret.startswith('(') else ret)
                ops.append(f'{"+" if vis=="public" else "-"} {n}('+', '.join(arguments)+')'+suffix)
        classes[name]={'base':base,'attrs':attrs,'events':events,'ops':ops}
        sources[name]=str(file.relative_to(ROOT)).replace('\\','/')
assert set(classes)=={'BuildingModel','BuildingPlacement','BuildingPreview','BuildingShapeUnit','GridManager','BuildingGridCell','InputManager','BuildingUI','BuildingData'}
assert sum(len(v['ops']) for v in classes.values())==39, [(k,len(v['ops'])) for k,v in classes.items()]
assert sum(len(v['events']) for v in classes.values())==4
(TMP/'uml-model.json').write_text(json.dumps(classes,indent=2),encoding='utf-8')

c=canvas.Canvas(str(OUT/'Bausystem-UML.pdf'),pagesize=(2384,1684))
c.setTitle('Bausystem - UML-Klassendiagramm und Code-Abgleich')
c.setAuthor('Projekt SeriousGame_AR')
W,H=2384,1684
def txt(x,y,s,size=13,font='Sans',color=INK):
    c.setFillColor(color); c.setFont(font,size); c.drawString(x,H-y,s)
def line(points,color=INK,dash=False,width=1.3):
    c.setStrokeColor(color); c.setLineWidth(width); c.setDash(6,4) if dash else c.setDash()
    p=c.beginPath(); p.moveTo(points[0][0],H-points[0][1])
    for x,y in points[1:]: p.lineTo(x,H-y)
    c.drawPath(p); c.setDash()
def label(x,y,s,size=12):
    wid=pdfmetrics.stringWidth(s,'Sans',size)
    c.setFillColor(white); c.rect(x-3,H-y-3,wid+6,size+5,fill=1,stroke=0)
    txt(x,y,s,size)
def arrow(points,kind='assoc',labels=()):
    line(points,dash=kind=='dependency')
    x,y=points[-1]; px,py=points[-2]; dx,dy=x-px,y-py; d=math.hypot(dx,dy); ux,uy=dx/d,dy/d
    n=12 if kind!='general' else 18
    a=(x-n*ux-6*uy,y-n*uy+6*ux); b=(x-n*ux+6*uy,y-n*uy-6*ux)
    if kind=='general':
        p=c.beginPath();p.moveTo(x,H-y);p.lineTo(a[0],H-a[1]);p.lineTo(b[0],H-b[1]);p.close()
        c.setFillColor(white);c.drawPath(p,fill=1,stroke=1)
    else: line([a,(x,y),b])
    if kind=='composition':
        x,y=points[0]; nx,ny=points[1];d=math.hypot(nx-x,ny-y);ux,uy=(nx-x)/d,(ny-y)/d
        pts=[(x,y),(x+10*ux-5*uy,y+10*uy+5*ux),(x+20*ux,y+20*uy),(x+10*ux+5*uy,y+10*uy-5*ux)]
        p=c.beginPath();p.moveTo(pts[0][0],H-pts[0][1])
        for xx,yy in pts[1:]:p.lineTo(xx,H-yy)
        p.close();c.setFillColor(INK);c.drawPath(p,fill=1,stroke=1)
    for item in labels:label(*item)

def wrapped(s,width,size):
    # Break only long operation parameter lists, preserving each signature.
    if pdfmetrics.stringWidth(s,'Mono',size)<=width:return [s]
    pieces=s.split(', '); result=[]; cur=''
    for p in pieces:
        trial=cur+(', ' if cur else '')+p
        if cur and pdfmetrics.stringWidth(trial,'Mono',size)>width:
            result.append(cur+',');cur='    '+p
        else:cur=trial
    result.append(cur)
    assert all(pdfmetrics.stringWidth(t,'Mono',size)<=width for t in result),(s,width)
    return result

boxes={}
def box(name,x,y,w,data=None,size=13):
    data=data or classes[name]; groups=[('Attribute / Properties',data['attrs']),('C#-Events',data['events']),('Operationen',data['ops'])]
    groups=[(title,items) for title,items in groups if items]
    rows=[(title,[v for item in items for v in wrapped(item,w-28,size)]) for title,items in groups]
    h=44+sum(28+len(items)*(size+6)+10 for _,items in rows)
    if not groups:h=70
    c.setFillColor(white);c.setStrokeColor(INK);c.setLineWidth(1.2);c.rect(x,H-y-h,w,h,fill=1,stroke=1)
    c.setFillColor(PALE);c.rect(x,H-y-44,w,44,fill=1,stroke=0)
    txt(x+14,y+29,name,19,'Bold')
    yy=y+44
    for title,items in rows:
        line([(x,yy),(x+w,yy)],width=.8);txt(x+14,yy+19,title,10,'Sans',GRAY);yy+=32
        for s in items:txt(x+14,yy+size-3,s,size,'Mono');yy+=size+6
        yy+=6
    boxes[name]=(x,y,w,h)
    return h

def header(k,title,subtitle):
    txt(45,43,'SERIOUSGAME_AR  /  SOFTWARESTRUKTUR',12,'Bold',BLUE)
    txt(45,83,title,29,'Bold');txt(45,111,subtitle,13,color=GRAY)
    txt(W-200,43,k,12,'Bold',BLUE)
def footer(n):
    line([(45,H-43),(W-45,H-43)],color=GRAY,width=.5)
    text='Code-Stand: 08.11.2026 | OMG UML 2.5.1, Dezember 2017'
    if W>700:text+=' | Vollständigkeit: eigene Deklarationen des Bausystems'
    txt(45,H-24,text,9 if W<700 else 10,color=GRAY)
    txt(W-100,H-24,str(n),10,color=GRAY)

header('ANSICHT 1 / 4','Bausystem - vollständige Klassen und fachliche Beziehungen','Alle 9 Projektklassen, 39 Operationen, 4 C#-Events und der verschachtelte Enum. Vererbung und externe Typen: Ansicht 2.')
box('InputManager',50,150,650)
box('BuildingUI',850,150,650)
box('BuildingData',1720,150,610)
box('BuildingPlacement',50,680,650)
box('BuildingPreview',850,680,650)
box('BuildingModel',1720,680,610)
box('GridManager',50,1250,950,size=12)
box('BuildingGridCell',1110,1320,550,size=12)
box('BuildingShapeUnit',1850,1320,480)
enumdata={'attrs':['None = 0','POSITIVE = 1','NEGATIVE = 2'],'events':[],'ops':[]}
box('«enumeration» BuildingPreview::BuildingState',1720,1000,610,enumdata,size=13)
# Each displayed field role is the same property as the attribute in its class box.
arrow([(150,680),(150,boxes['InputManager'][1]+boxes['InputManager'][3])],labels=[(160,590,'- inputManager  0..1'),(158,662,'0..*')])
arrow([(430,680),(430,625),(930,625),(930,boxes['BuildingUI'][1]+boxes['BuildingUI'][3])],labels=[(950,616,'- buildingUI  0..1'),(438,662,'0..*')])
arrow([(700,760),(850,760)],labels=[(714,746,'- currentPreview'),(711,782,'0..*'),(807,782,'0..1')])
arrow([(700,835),(850,835)],labels=[(714,821,'- previewPrefab'),(711,857,'0..*'),(807,857,'0..1')])
arrow([(650,680),(650,605),(1770,605),(1770,boxes['BuildingData'][1]+boxes['BuildingData'][3])],labels=[(1510,596,'- currentBuilding'),(1780,315,'0..1'),(657,663,'0..*')])
arrow([(1500,200),(1720,200)],labels=[(1510,190,'- building1x1'),(1510,215,'0..*'),(1675,215,'0..1')])
arrow([(1500,240),(1720,240)],labels=[(1510,230,'- building2x2'),(1510,255,'0..*'),(1675,255,'0..1')])
arrow([(1500,280),(1720,280)],labels=[(1510,270,'- building3x3'),(1510,295,'0..*'),(1675,295,'0..1')])
arrow([(1440,680),(1440,645),(1850,645),(1850,boxes['BuildingData'][1]+boxes['BuildingData'][3])],labels=[(1510,636,'- buildingData'),(1860,315,'0..1'),(1448,674,'0..*')])
arrow([(2090,boxes['BuildingData'][1]+boxes['BuildingData'][3]),(2090,680)],labels=[(2100,665,'+ Model  0..1'),(2100,315,'0..*')])
arrow([(1500,765),(1720,765)],labels=[(1520,751,'- buildingModel'),(1512,788,'0..*'),(1672,788,'0..1')])
arrow([(700,1000),(790,1000),(790,665),(1990,665),(1990,680)],kind='dependency',labels=[(1040,655,'erzeugt endgültiges Modell')])
arrow([(150,boxes['BuildingPlacement'][1]+boxes['BuildingPlacement'][3]),(150,1250)],labels=[(160,1210,'- grid  0..1'),(158,1187,'0..*')])
arrow([(1000,boxes['BuildingPreview'][1]+boxes['BuildingPreview'][3]),(1000,1180),(870,1180),(870,1250)],labels=[(1010,1155,'0..*'),(880,1235,'- gridManager  0..1')])
arrow([(1000,1480),(1110,1480)],kind='composition',labels=[(1010,1457,'- grid'),(1008,1501,'1'),(1064,1501,'0..*')])
arrow([(1550,1320),(1550,1180),(1640,1180),(1640,925),(1800,925),(1800,boxes['BuildingModel'][1]+boxes['BuildingModel'][3])],labels=[(1560,1300,'0..*'),(1812,950,'- building  0..1')])
arrow([(2270,boxes['BuildingModel'][1]+boxes['BuildingModel'][3]),(2270,950),(2370,950),(2370,1350),(2330,1350)],labels=[(2190,1305,'- shapeUnits'),(2335,1370,'0..*'),(2280,940,'0..*')])
arrow([(1500,1015),(1620,1015),(1620,1040),(1720,1040)],kind='dependency',labels=[(1520,1000,'verwendet Enum')])
txt(1720,1200,'BuildingState ist öffentlich in BuildingPreview verschachtelt.',12,color=GRAY)
txt(1720,1220,'ShapeUnits sind Referenzen; kein belegtes exklusives Eigentum.',12,color=GRAY)
txt(1110,1580,'0..1 berücksichtigt ungesetzte Referenzen. Zugewiesene Pflichtreferenzen:',12,color=GRAY)
txt(1110,1600,'siehe Initialisierungsbedingungen und Notation in Ansicht 2.',12,color=GRAY)
footer(1);c.showPage()

# A3 supplementary view: inheritance, value type mapping and normative notation.
W,H=1191,842;c.setPageSize((W,H))
header('ANSICHT 2 / 4','Vererbung, Typen und Notation','Ergänzende Sicht auf dasselbe Modell. Leere Kompartimente und geerbte Framework-Member sind ausgeblendet.')
def simple(name,x,y,w=205,h=42):
    c.setStrokeColor(INK);c.setFillColor(white);c.rect(x,H-y-h,w,h,fill=1,stroke=1);txt(x+10,y+27,name,12,'Bold')
simple('UnityEngine::MonoBehaviour',395,155,290)
children=['InputManager','BuildingUI','BuildingPlacement','BuildingPreview','BuildingModel','BuildingShapeUnit','GridManager']
for i,n in enumerate(children):
    x=40+i*162;simple(n,x,290,150);line([(x+75,290),(x+75,250)])
line([(115,250),(1087,250)]);arrow([(540,250),(540,197)],kind='general')
simple('UnityEngine::ScriptableObject',45,390,290);simple('BuildingData',100,485,180)
arrow([(190,485),(190,432)],kind='general')
simple('BuildingGridCell',420,390,220)
txt(420,456,'Normale C#-Klasse; kein MonoBehaviour.',11,color=GRAY)
simple('«dataType» GridCoordinates',790,390,320,100)
txt(805,447,'+ x: int',12,'Mono');txt(805,468,'+ z: int',12,'Mono')
txt(790,515,'Notationsalias für C# (int x, int z).',11,color=GRAY)
txt(790,533,'Keine zusätzliche Klasse im Projekt.',11,color=GRAY)
style=ParagraphStyle('body',fontName='Sans',fontSize=10.2,leading=14,textColor=INK)
def para(x,y,w,s,style=style):
    p=Paragraph(s,style);ww,hh=p.wrap(w,1000);p.drawOn(c,x,H-y-hh);return hh
para(45,565,535,'<b>UML-Schreibweise.</b> + öffentlich, - privat. Attribute: name: Typ. Operationen: name(parameter: Typ): Rückgabetyp. Ohne Rückgabetyp bedeutet hier C# void. shapeUnits ist ein C#-Array, renderers eine List&lt;Renderer&gt;, grid ein 2D-Array. Diese Attribute sind als mehrwertige Properties dargestellt; Rückgabetypen bleiben C#-Typen.')
para(45,643,535,'<b>Beziehungen.</b> Durchgezogene Linie mit offenem Pfeil: Navigation zur referenzierten Klasse. Gefüllte Raute: Komposition des Grids aus seinen Zellen. Gestrichelter Pfeil: Nutzung/Erzeugung. Hohles Dreieck: Generalisierung zur Basisklasse. Kreuzungen ohne Punkt sind keine Verbindung.')
para(45,721,535,'<b>Mehrfachdarstellung.</b> Rollen an Linien und gleichnamige Attribute bezeichnen dieselbe Property. Quelle 0..* erlaubt mehrere Referenzhalter; Ziel 0..1 erlaubt null. Grid-Zellen haben genau einen erzeugenden GridManager. Keine Komposition wird allein aus einer Referenz abgeleitet.')
para(620,565,525,'<b>C#-Abbildung.</b> Das optionale Kompartiment C#-Events zeigt Delegat-Events mit ihrem Action-Typ; es sind keine UML-Signale oder Receptions. BuildingData-Properties besitzen öffentliche Getter und private Setter; die C#-Constraint dokumentiert dies, ohne sie als vollständig readOnly auszugeben.')
para(620,643,525,'<b>Externe Typen.</b> Camera, MeshCollider, GameObject, Renderer, Material und Button sind Framework-Referenzen. Vector2/Vector3, Touch, Action&lt;T&gt;, List&lt;T&gt;, Arrays und primitive C#-Typen werden als benannte externe Typen verwendet; ihre internen Mitglieder gehören nicht zum Bausystem.')
para(620,721,525,'<b>Initialisierung.</b> Inspector-Referenzen müssen vor ihrer Verwendung gesetzt sein; Preview.Setup und GridManager.Start müssen vor Bauprüfungen gelaufen sein. Diese Laufzeitvorbedingungen sind keine im C#-Typ garantierte Multiplizität 1. Standardwerte aus Feldinitialisierern sind angegeben.')
footer(2);c.showPage()

W,H=595.276,841.89;c.setPageSize((W,H))
def reporthead(num,title):
    txt(40,38,'SERIOUSGAME_AR / UML-ABGLEICH',9,'Bold',BLUE)
    txt(40,77,title,22,'Bold');txt(40,99,'08.11.2026 | Tatsächlicher Quellcode und eingereichte Übersicht',9,color=GRAY)
    txt(490,38,f'{num} / 4',9,'Bold')
def reportitem(y,title,text):
    txt(40,y,title,11,'Bold');hh=para(40,y+9,515,text);return y+hh+29
reporthead(3,'Korrekturen an deiner Übersicht')
y=128
corrections=[
('1  Fehlende Klassen und Member','BuildingData und BuildingShapeUnit fehlen in der PDF. Ergänzt sind außerdem BuildingUI.buildingButton, BuildingPreview.state, vollständige Typen, Sichtbarkeiten, Event-Signaturen und Beziehungen. Building.cs existiert im aktuellen Stand nicht mehr.'),
('2  Positionen sind keine ShapeUnit-Objekte','BuildingModel.GetAllBuildingPositions() liefert List&lt;Vector3&gt;, nicht Vector3 und nicht List&lt;BuildingShapeUnit&gt;. GridManager.CanBuild() und SetBuildig() erhalten diese Weltpositionen.'),
('3  SetBuildig: Richtung und Wirkung','Die Methode heißt im Code SetBuildig. Sie rechnet Weltpositionen in Grid-Koordinaten um und hinterlegt die Referenz auf ein bereits erzeugtes BuildingModel in den Zellen. Sie erzeugt, verschiebt oder platziert selbst kein sichtbares Objekt.'),
('4  Zellenecke und Zellmitte','GridToWorldPosition(x, z) liefert die Zellenecke. Die zweite, in deiner PDF ebenfalls so benannte Methode heißt tatsächlich GridToWorldCenterPosition(x, z) und liefert die Zellmitte.'),
('5  Collider-Prüfung wird gespeichert','IsCellOnCollider() wird beim Rasteraufbau aufgerufen; CanBuild() und BuildingGridCell.IsEmpty() verwenden später die gespeicherte Bebaubarkeit. Sie führen selbst keine neuen Collider-Raycasts aus. OnDrawGizmos() zeichnet nur bebaubare Zellen.'),
('6  BuildingFinished gehört zur UI','BuildingPreview besitzt keine Methode BuildingFinished(). Diese Methode existiert ausschließlich in BuildingUI. SetPreview() sammelt auch keine Renderer neu: Es nutzt die in Setup() gespeicherte Liste und ändert Materialien nur bei Zustandswechsel.'),
('7  Touch-Ablauf präzisiert','touchStartPosition ist eine Bildschirmposition und kein Welt-Trefferpunkt. OnTouchMoved() verschiebt erst nach Überschreiten von tapThreshold; OnTouchEnded() platziert nur bei einem Tap nach erkanntem Vorschaukontakt. TrackFingerPosition() läuft nur bei Aufruf, nicht eigenständig jedes Frame.'),
('8  Auswahl und Abschluss','GetSelectedBuilding() übergibt Daten und Grid an Preview.Setup(). PlaceBuilding() setzt currentPreview und currentBuilding auf null und isDragging auf false; es setzt nicht alle Referenzen zurück. Die wiederverwendbaren Inspector-Referenzen bleiben erhalten.')]
for title,body in corrections:y=reportitem(y,title,body)
assert y<790,y
footer(3);c.showPage()

reporthead(4,'Quelle, Umfang und Prüfhinweise')
y=130
y=reportitem(y,'Die angegebene Abbildung zeigt Komponenten','OMG, Unified Modeling Language 2.5.1, Dezember 2017, Abschnitt 11.6.5, Abbildung 11.39, gedruckte Seite 211 (PDF-Seite 253), zeigt Komponenten und ihre allgemeinen Abhängigkeiten. Sie ist keine Vorlage für Klassen mit Attributen und Operationen.')
y=reportitem(y,'Passende Grundlage für dieses Klassendiagramm','Verwendet wurden derselben Spezifikation: 9.2.4 (Classifier-Darstellung und Generalisierung), 9.5.4 (Properties), 9.6.4 (Operationen), 11.4.4 und 11.4.5 / Abbildung 11.16 (Klassen, Seiten 195-196) sowie 11.5.4 (Assoziationen). Leere Kompartimente sind unterdrückt. Die C#-Abbildungsregeln sind ausdrücklich in Ansicht 2 dokumentiert.')
y=reportitem(y,'Vollständigkeit und Abgrenzung','Enthalten sind alle 9 aktuellen eigenen Klassen des Bausystems mit allen deklarierten Feldern/Properties, 39 Methoden und 4 Events sowie BuildingState. Eingeschlossen sind Input und Gebäude-UI. Editor-Farben, Testskript und Unity-Vorlagenskript sind fachlich außerhalb des Bausystems; Framework-Member und Compiler-generierte Accessoren sind nicht expandiert.')
y=reportitem(y,'Verhaltensdetails - nicht stillschweigend korrigiert','CanPlace() akzeptiert über CanBuild() auch eine leere Positionsliste, während Update() diese ablehnt. TouchPhase.Canceled wird als Ende-Event weitergegeben und kann damit eine Tap-Platzierung auslösen. PlaceBuilding() übernimmt die Rotation des Modell-Roots, nicht eine separate Wrapper-Rotation. Diese Punkte betreffen Verhalten, nicht zusätzliche UML-Methoden.')
y=reportitem(y,'Referenzen und Eigentum','Die Vorschau und ihr Modell werden zwar als Unity-Kinder erzeugt; die öffentlich erneut aufrufbaren Setup-/Auswahlmethoden erzwingen aber keine allgemeine exklusive Zuordnung im Klassentyp. Deshalb werden diese Referenzen konservativ als Assoziationen dargestellt. Nur die im Grid erzeugten und gehaltenen BuildingGridCells sind als Komposition modelliert.')
y=reportitem(y,'Prüfverfahren','Die eingereichte PDF wurde gelesen und gerendert; Attribute, Signaturen und Sichtbarkeiten wurden gegen die aktuellen C#-Dateien abgeglichen. Der Quellcode wurde nicht geändert. Diese Prüfung umfasst keinen Unity-Playtest und keine Prüfung der Inspector-Zuweisungen in Szenen oder Prefabs.')
y=reportitem(y,'Quellen','Eingereicht: C:/Users/nschu/Downloads/UML-Klassendiagramm.pdf.<br/>Projekt: Assets/_Project/Scripts/{Buildings/Placement,Grid/Systems,Input,UI} und Assets/_Project/ScriptableObjects/Buildings/SoScripts/BuildingData.cs.<br/>OMG (2017): <link href="https://www.omg.org/spec/UML/2.5.1/PDF" color="#234f70">https://www.omg.org/spec/UML/2.5.1/PDF</link>.')
assert y<790,y
footer(4);c.save()

# Editable textual counterpart of the complete diagram.
puml=['@startuml','title Bausystem - Code-Stand 08.11.2026','skinparam classAttributeIconSize 0','skinparam shadowing false','hide empty members','left to right direction']
for name,d in classes.items():
    puml.append('class '+name+' {')
    for title,key in [('Attribute / Properties','attrs'),('C#-Events','events'),('Operationen','ops')]:
        if d[key]:puml.append('  -- '+title+' --');puml += ['  '+s for s in d[key]]
    puml.append('}')
puml+=['enum "BuildingPreview::BuildingState" as BuildingState {','  None = 0','  POSITIVE = 1','  NEGATIVE = 2','}', 'class "UnityEngine::MonoBehaviour" as MonoBehaviour','class "UnityEngine::ScriptableObject" as ScriptableObject','class GridCoordinates <<datatype>> {','  + x: int','  + z: int','}']
for n,d in classes.items():
    if d['base']:puml.append(f'{n} --|> {d["base"]}')
rels=[('BuildingPlacement','InputManager','inputManager'),('BuildingPlacement','BuildingUI','buildingUI'),('BuildingPlacement','BuildingPreview','previewPrefab'),('BuildingPlacement','BuildingPreview','currentPreview'),('BuildingPlacement','BuildingData','currentBuilding'),('BuildingPlacement','GridManager','grid'),('BuildingPreview','BuildingData','buildingData'),('BuildingPreview','BuildingModel','buildingModel'),('BuildingPreview','GridManager','gridManager'),('BuildingData','BuildingModel','Model'),('BuildingGridCell','BuildingModel','building')]
rels += [('BuildingUI','BuildingData',s) for s in ['building1x1','building2x2','building3x3']]
for a,b,role in rels:puml.append(f'{a} "0..*" --> "0..1" {b} : {role}')
puml += ['BuildingModel "0..*" --> "0..*" BuildingShapeUnit : shapeUnits','GridManager "1" *--> "0..*" BuildingGridCell : grid','BuildingPlacement ..> BuildingModel : erzeugt','BuildingPreview ..> BuildingState : verwendet','GridManager ..> GridCoordinates : Rückgabetyp','note right of GridCoordinates : Alias für C# (int x, int z); keine Projektklasse.','note right of BuildingState : Öffentlich in BuildingPreview verschachtelt.','legend bottom','+ public / - private. Kein Rückgabetyp = void.','C#-Events: optionale Implementierungsdarstellung, keine UML-Signale.','Property-Constraints dokumentieren C#-Accessor-Sichtbarkeit.','Referenzrollen und Attribute benennen dieselben Properties.','Framework-Typen werden nicht intern expandiert.','Quelle: OMG UML 2.5.1, 9.2.4, 9.5.4, 9.6.4, 11.4.4-5, 11.5.4.','11.6.5 / Abb. 11.39 zeigt ein Komponentendiagramm, kein Klassendiagramm.','endlegend','@enduml']
(OUT/'Bausystem-UML.puml').write_text('\n'.join(puml),encoding='utf-8')
manifest={str(f.relative_to(ROOT)).replace('\\','/'):hashlib.sha256(f.read_bytes()).hexdigest() for f in files}
(TMP/'source-hashes.json').write_text(json.dumps(manifest,indent=2))
doc=pdfium.PdfDocument(str(OUT/'Bausystem-UML.pdf'))
for i in range(len(doc)):
    scale=.8 if i==0 else 1.5
    doc[i].render(scale=scale).to_pil().save(str(TMP/f'final-{i+1}.png'))
print('Classes:',len(classes),'Operations:',sum(len(v['ops']) for v in classes.values()),'Events:',sum(len(v['events']) for v in classes.values()))
print('Boxes:',boxes)
print('Output:',OUT/'Bausystem-UML.pdf')
