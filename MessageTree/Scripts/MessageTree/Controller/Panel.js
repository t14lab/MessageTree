t14Lab.MessageTree.Panel = function (context) {
    if (context) {
        this.isScrollable = context.isScrollable;
        this.panelName = context.panelName;
        this.type = context.type;
        this.managePanelWithName = context.parentPanel;
        this.parentPanel = context.parentPanel;
        this.selectedId = context.selectedId;

        this.gridType = context.gridType;

        this.participants = context.participants;
        this.options = context.participants;
        this.cleared = false;
    } else {
        this.isScrollable = false;
        this.panelName = "defaultPanel";
        this.type = "";
        this.managePanelWithName = "";
        this.parentPanel = "";
        this.gridType = "defaultType";
        this.cleared = false;
    }
}
