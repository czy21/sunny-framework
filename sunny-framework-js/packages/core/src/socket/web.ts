export interface Options {
    server: string
    heartbeat?: {
        interval: number
        timeout: number
    }
    maxReconnectCount?: number
}

export class Socket {
    ws: WebSocket
    options: Options | any = {
        server: '',
        heartbeat: {
            interval: 20,
            timeout: 5
        },
        maxReconnectCount: 5
    }
    reConnectCount: number = 0
    heartTimer: number
    serverTimer: number

    onclose: ((ev: CloseEvent) => any) | null;
    onerror: ((ev: Event) => any) | null;
    onmessage: ((ev: MessageEvent) => any) | null;
    onopen: ((ev: Event) => any) | null;

    constructor(options: Options) {
        this.options = {...this.options, ...options}
    }

    connect() {

        this.ws = new WebSocket(this.options.server)

        this.ws.onopen = ev => {
            this.reConnectCount = 0
            if (this.onopen) {
                this.onopen(ev)
            } else {
                this.ping()
            }
        }

        this.ws.onclose = ev => {
            if (this.onclose) {
                this.onclose(ev)
            } else {
                this.reConnect()
            }
        }

        this.ws.onerror = ev => {
            if (this.onerror) {
                this.onerror(ev)
            } else {
                this.reConnect()
            }
        }

        this.ws.onmessage = ev => {
            if (ev.data === 'pong') {
                this.ping()
            } else {
                this.onmessage && this.onmessage(ev)
            }
        }
    }

    disConnect() {
        this.ws.close()
    }

    reConnect() {

        if (this.reConnectCount >= this.options.maxReconnectCount) {
            return
        }

        this.reConnectCount++

        setTimeout(() => this.connect(), 1000 * this.reConnectCount)

    }

    ping() {
        this.reset()
        this.heartTimer = setTimeout(() => {
            this.ws.send("ping")
            this.serverTimer = setTimeout(() => this.ws.close(), this.options.heartbeat?.timeout * 1000)
        }, this.options.heartbeat?.interval * 1000)
    }

    reset() {
        this.heartTimer && clearTimeout(this.heartTimer)
        this.serverTimer && clearTimeout(this.serverTimer)
    }

    send(data: string | ArrayBufferLike | Blob | ArrayBufferView): void {
        this.ws.send(data)
    }
}