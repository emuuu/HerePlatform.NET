window.BlazorDocs = {
    highlightAll: function () {
        if (typeof Prism !== 'undefined') {
            Prism.highlightAll();
        }
    },

    highlightElement: function (element) {
        if (typeof Prism !== 'undefined' && element) {
            Prism.highlightElement(element);
        }
    },

    copyToClipboard: function (text) {
        if (navigator.clipboard) {
            return navigator.clipboard.writeText(text);
        }
    }
};
