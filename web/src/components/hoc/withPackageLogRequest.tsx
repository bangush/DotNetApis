import * as React from 'react';

import { PackageInjectedProps, Hoc, ExtendingHoc } from '.';
import { State } from '../../reducers';
import { PackageLogState } from '../../ducks/packageLog';

// TODO: Consider removing PackageInjectedProps
export interface PackageLogRequestInjectedProps extends PackageInjectedProps {
    pkgLogRequestStatus: PackageLogState;
}
export type PackageLogRequestRequiredProps = State & PackageInjectedProps;

function createWithPackageLogRequest<TProps>(): Hoc<TProps & PackageLogRequestRequiredProps, TProps & PackageLogRequestInjectedProps> {
    return Component => props => {
        const request = props.packageLog.packageLogs[props.pkgRequestStatus.normalizedPackageKey!];
        return <Component {...props} pkgLogRequestStatus={request}/>;
    };
}

/** Takes the package request parameters and injects `PackageLogRequestInjectedProps` */
export const withPackageLogRequest : ExtendingHoc<PackageLogRequestInjectedProps, PackageLogRequestRequiredProps> = createWithPackageLogRequest();
