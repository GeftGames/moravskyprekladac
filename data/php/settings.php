<!-- Settings-->
<div id="nav" class="navTrans">

    <!-- Current translation, only transtation page -->
    <div>
        <p style="display:inline-block;text-decoration: underline;" id="textTranslation">Překlad</p>
        <br>

        <!-- Copy current translation -->
        <button class="button" id="textCopyThisTrans" onclick="CopyLink()">Kopírovat odkaz na&nbsp;překlad</button>
        <br>
        
        <!-- Save current translation -->
        <button class="button" id="textSaveTrans" onclick="SaveTrans()">Uložit tento&nbsp;překlad</button>
        <div style="margin: 10px 0 10px 0;"></div>
    </div>

    <!-- General settings-->
    <div>
        <p style="display:inline-block;text-decoration: underline" id="textSettings">Obecná nastavení</p>
        <div id="sisetting">
        <div style="display: flex; justify-content: space-between; align-items: center;">
            
        <!-- Jazyk -->
            <p style="flex-direction: column">
                <span id="textWeblanguage">Jazyk webu</span>
                <span class="moreinfo">Language</span>
            </p>
            <select id="lang" style="margin: 5px;text-align: center;" onchange="language=this.options[this.selectedIndex].value; localStorage.setItem('setting-language', language); SetLanguage();">
                <option id="textDefault" style='font-weight: bold' value="default">Výchozí</option>
                <option value="en">English</option>
                <option value="cs">Čeština</option>
                <option value="mo">Po moravsky</option>
                <option value="pl">Polski</option>
                <option value="ha">Hanácke</option>
                <option value="de">Deutsch</option>
                <option value="sk">Slovenčina</option>
                <option value="jp">日本語</option>
            </select>
        </div>

        <!-- Styl -->
        <div style="display: flex; justify-content: space-between; align-items: center">
            <p style="flex-direction: column">
                <span id="textTheme">Motiv</span>
            </p>
            <select id="theme" style="margin: 5px;text-align: center;" onchange="SetTheme();">
                <option id="textDefault2" style='font-weight: bold' value="default">Výchozí</option>
                <option id="textLight" value="light">Svjetlý</option>
                <option id="textDark" value="dark">Tmavý</option>
                <option id="textNightDark" value="nightdark">Night Tmavý</option>
                <option id="textNightDark" value="blue">Modrý</option>
            </select>
        </div>

        <!-- Dev -->
        <div class="settingItem">
            <p>
                <span id="textMoreInfo">Rozšířené informace</span>
                <span class="moreinfo" id="textMoreInfoDev">Pro&nbsp;vývojáře</span>
            </p>

            <label class="switch">
                <input type="checkbox" id="dev" onchange="ChangeDev()" >
                <span class="slider round"></span>
            </label>
        </div>
    </div>

    <!-- Translate options-->
    <div>
        <!-- Automatic translate -->
        <p style="display:inline-block;text-decoration: underline" id="textSettings">Nastavení překladu</p>
        <div class="settingItem">
            <p id="textAutoTranslate">Automatický překlad</p>

            <label class="switch">
                <input id="manual" type="checkbox" checked onchange="ChangeManual()">
                <span class="slider round"></span>
            </label>
        </div>

        <!-- Text stylizate -->
        <div class="settingItem">
            <p id="textMark">Zvýraznění překladu</p>

            <label class="switch">
                <input id="styleOutput" type="checkbox" onchange="ChangeStylizate()">
                <span class="slider round"></span>
            </label>
        </div>

        <!-- Clear saved -->
        <div class="settingItem">
            <p id="textSaved">Uložené překlady</p>
            <button id="textRemove" class="button" onclick="RemoveTrans()">Vymazat</button>
        </div>

        <!-- Nedělané funkce -->
        <div class="settingItem">
            <p id="textTesting">Testovací fce</p>

            <label class="switch">
                <input id="testingFunc" type="checkbox" onchange="ChangeTesting()">
                <span class="slider round"></span>
            </label>
        </div>
    </div>

    <!-- Poznámka -->
    <div>
        <p style="display:inline-block;text-decoration: underline" id="textSettings">Informace</p>
        <br>
        <p style="display:inline-block;text-decoration: underline;" id="textPCSaving">Ukládání do počítače</p>
        <p style="display:inline-block" id="textCookies">Tato stránka nepoužívá cookies. K&nbsp;ukládání do&nbsp;nastavení se používá localStorage.</p>
    </div>
</div>