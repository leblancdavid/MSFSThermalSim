class ThermalSimPanelPanel extends TemplateElement {
    constructor() {
        super(...arguments);
    }
    connectedCallback() {
        super.connectedCallback();
        this.m_MainDisplay = document.querySelector("#MainDisplay");
        this.m_MainDisplay.classList.add("hidden");
        this.m_Footer = document.querySelector("#Footer");
        this.m_Footer.classList.add("hidden");

        const textBuffer = document.getElementById("textBuffer");
        const lockBuffer = document.getElementById("lockBuffer");

        this.ingameUi = this.querySelector('ingame-ui');
        this.iframeElement = document.getElementById("ThermalSimIframe");

        if (this.ingameUi) {
            this.ingameUi.addEventListener("panelActive", (e) => {
                console.log('panelActive');
                self.panelActive = true;
                if (self.iframeElement) {
                    self.iframeElement.src = 'https://localhost:7187/swagger/index.html';
                }
            });
            this.ingameUi.addEventListener("panelInactive", (e) => {
                console.log('panelInactive');
                self.panelActive = false;
                if (self.iframeElement) {
                    self.iframeElement.src = '';
                }
            });
            this.ingameUi.addEventListener("onResizeElement", () => {
                //self.updateImage();
            });
            this.ingameUi.addEventListener("dblclick", () => {
                if (self.m_Footer) {
                    self.m_Footer.classList.remove("hidden");
                }
			});
        }

    }
    initialize() {
    }
    disconnectedCallback() {
        super.disconnectedCallback();
    }
    updateImage() {
    }
}
window.customElements.define("ingamepanel-thermalsim", ThermalSimPanelPanel);
checkAutoload();