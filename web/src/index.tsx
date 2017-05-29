import * as React from "react";
import * as ReactDOM from "react-dom";
import { client } from './logic/log-listener';

import { Hello } from "./components/Hello";

ReactDOM.render(
    <Hello compiler="TypeScript" framework="React" />,
    document.getElementById("example")
);

client.channels.get("log:test").subscribe((message) => 
{
    console.log(message);
});