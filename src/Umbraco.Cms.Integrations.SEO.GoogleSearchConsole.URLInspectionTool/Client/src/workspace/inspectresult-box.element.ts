import { html, css, customElement, property, when } from "@umbraco-cms/backoffice/external/lit";
import { UmbLitElement } from "@umbraco-cms/backoffice/lit-element";

@customElement("inspectresult-box")
export class InspectResultBoxElement extends UmbLitElement {
    @property({ type: String })
    public headline: string = "";

    @property({ type: String })
    public headlineSlot: string = "";

    @property({ type: String })
    public link: string = "";

    @property({ attribute: false })
    public data: Record<string, unknown> = {};

    get keyValuePairs(): Array<{ key: string; value: unknown }> {
        return Object.entries(this.data).map(([key, value]) => ({ key, value }));
    }

    @property({ type: String })
    public content: string = "";

    #renderLink() {
        return html`<a href=${this.link} target="_blank">${this.link}</a>`;
    }

    #renderData() {
        return html`${this.keyValuePairs.map(({ key, value }) => html`<p><strong>${key}:</strong> ${value}</p>`)}`;
    }

    render() {
        return html`
            <uui-box .headline=${this.headline}>
                <div slot="headline">
                    <h5>${this.headlineSlot}</h5>
                </div>
                <div>
                    ${when(this.link.length > 0, () => this.#renderLink())}
                    ${when(this.keyValuePairs.length > 0, () => this.#renderData())}
                </div>
            </uui-box>
            <br />
        `;
    }

    static styles = [
        css`
          h5 {
            margin: 0;
            font-weight: normal;
            color: var(--uui-color-text-alt);
          }
        `
    ];
}

declare global {
    interface HTMLElementTagNameMap {
        "inspectresult-box": InspectResultBoxElement;
    }
}