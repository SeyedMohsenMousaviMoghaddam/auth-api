const styles = `
#swagger-ui {
    display: flex;
}

.swagger-container{
    width: 83%;
}

.swagger-cs-menu {
    width: 17%;
}

.topbar, .info {
    display: none;
}

.operation-tag-content, .models  {
    direction: ltr;
}

.scheme-container {
    top: 0;
    background-color: #fff;
    z-index: 1000;
    padding: 10px 0 !important;
}

.swagger-cs-menu {
    height: 100vh;
    overflow-y: auto;
    top: 0;
}

.swagger-cs-menu__list {
    list-style: none;
    padding: 0;
    margin: 0;
}

.swagger-cs-menu__item {
    margin: 0 7px;
    color: #3b4151;
    border-bottom: 1px solid rgba(59,65,81,.3);
    transition: 0.3s ease;
    background-color: transparent;
    cursor: pointer;
}

.swagger-cs-menu__link {
    color: inherit;
    transition: 0.3s ease;
    text-decoration: none;
    display: block;
    padding: 10px 15px;
 }

.swagger-cs-menu__item:hover, .swagger-cs-menu__item--active {
    background-color: #49cc90;
    color: #fff;
    text-decoration: none;
}

.auth-wrapper {
    justify-content: center !important;
}

.swagger-ui .opblock-tag {
	font-size: 19px;
}

::-webkit-scrollbar{
    width: 7px;
    height: 7px;
}

::-webkit-scrollbar-thumb{
    background-color: #49cc90;
    border-radius: 60px;
}
`;

let menuItems = [];

window.addEventListener('load', function () {
    this.setTimeout(() => {
        this.document.head.insertAdjacentHTML(
            'beforeend',
            `<style>${styles}</style>`
        );
        document.querySelectorAll('.opblock-tag').forEach(function (i) {
            i.click();
            const id = i.getAttribute('id');
            const title = i.getAttribute('data-tag');
            menuItems.push({
                id,
                title,
            });
        });
        document
            .querySelector('#swagger-ui')
            .insertAdjacentHTML(
                'afterbegin',
                `<div class="swagger-cs-menu swagger-ui"></div>`
            );
        const scheme = document.querySelector('.scheme-container');
        document.querySelector('.swagger-cs-menu').appendChild(scheme);
        document
            .querySelector('.swagger-cs-menu')
            .insertAdjacentHTML('beforeend', '<ul class="swagger-cs-menu__list"></ul>');
        const el = document.querySelector('.swagger-cs-menu__list');

        menuItems.forEach((i) => {
            el.insertAdjacentHTML(
                'beforeend',
                `
			<li class="swagger-cs-menu__item">
				<a class="swagger-cs-menu__link" href="#${i.id}">${i.title}</a>
			</li>
		`
            );
        });

        document.querySelectorAll('.swagger-cs-menu__link').forEach((i) => {
            i.addEventListener('click', () => {
                document.querySelectorAll('.opblock-tag').forEach(function (j) {
                    if (j.parentElement.classList.contains('is-open')) j.click();
                });
                document.querySelector(i.getAttribute('href')).click();
            });
        });

    }, 200)
});
