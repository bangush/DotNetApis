import * as React from "react";
import { LinkProps } from "react-router-dom";

import { ILocation, isCurrentPackageLocation, isDependencyLocation } from "../structure";
import { ReactFragment, FormatContext } from "./util";
import { PackageEntityLink, ReferenceEntityLink } from "../components/links";

export function locationLink(context: FormatContext, location: ILocation, content: ReactFragment, linkProps?: LinkProps): ReactFragment {
    const { pkg } = context;
    if (!location || !context.includeLinks)
        return content;
    else if (isCurrentPackageLocation(location))
        return <PackageEntityLink packageId={pkg.i} packageVersion={pkg.v} targetFramework={pkg.t} dnaid={location} linkProps={linkProps}>{content}</PackageEntityLink>;
    else if (isDependencyLocation(location))
        return <PackageEntityLink packageId={location.p} packageVersion={location.v} targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{content}</PackageEntityLink>;
    else
        return <ReferenceEntityLink targetFramework={pkg.t} dnaid={location.i} linkProps={linkProps}>{content}</ReferenceEntityLink>;
}