export function subscribeToOnInput(componentRef, inputRef) {
    resize(inputRef);

    window.onresize = e => {
        resize(inputRef);
    };

    inputRef.addEventListener("input",
        e => {
            resize(inputRef);
        });

    inputRef.onkeydown = e => {
        if (e.key !== "Tab") return;

        e.preventDefault();

        let textarea = e.target;
        let start = textarea.selectionStart;
        let end = textarea.selectionEnd;
        let value = textarea.value;

        textarea.value = value.substring(0, start) + "\t" + value.substring(end);
        textarea.selectionStart = textarea.selectionEnd = start + 1;
    };

    inputRef.onkeyup = async e => {
        await componentRef.invokeMethodAsync("OnInput", e.target.value);
    };
}


function resize(el) {
    let mainEl = document.querySelector("main");
    let scrollTop = mainEl.scrollTop;

    let mainElStyle = getComputedStyle(mainEl);

    el.style.height = "auto";
    let scrollHeight = el.scrollHeight;
    let parentHeight = parseInt(mainElStyle.height) - parseInt(mainElStyle.paddingTop) - 50;

    el.style.height = (scrollHeight < parentHeight / 2 ? parentHeight : scrollHeight + parentHeight / 2) + "px";
    mainEl.scrollTop = scrollTop;
}