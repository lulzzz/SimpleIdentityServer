﻿import React, { Component } from "react";
import { translate } from 'react-i18next';
import { ScimTab, OpenIdTab, AuthorizationTab } from './logsTabs';
import { withRouter } from 'react-router-dom';

class Logs extends Component {
    constructor(props) {
        super(props);
        this.navigate = this.navigate.bind(this);
        this.state = {
            tabName: null
        };
    }

    navigate(e, tabName) {
        e.preventDefault();
        this.setState({
            tabName: tabName
        });
        this.props.history.push('/logs/' + tabName);
    }

    render() {
        var self = this;
        const { t } = self.props;
        return (<div className="block">
            <div className="block-header">
                <h4>{t('logsTitle')}</h4>
                <i>{t('logsShortDescription')}</i>
            </div>
            <div className="container-fluid">
                <div className="row">
                    <div className="col-md-12">
                        <div className="card">
                            <div className="body">
                                <ul className="nav nav-tabs">
                                    <li className="nav-item">
                                        <a href="#" className="nav-link" onClick={(e) => self.navigate(e, "openid")}>{t('openid')}</a>
                                    </li>
                                    <li className="nav-item">
                                        <a href="#" className="nav-link" onClick={(e) => self.navigate(e, "scim")}>{t('scim')}</a>
                                    </li>
                                    <li className="nav-item">
                                        <a href="#" className="nav-link" onClick={(e) => self.navigate(e, "authorization")}>{t('authorization')}</a>
                                    </li>
                                 </ul>
                             </div>
                        </div>
                    </div>
                    <div className="col-md-12">
                        {self.state.tabName === 'openid' && (<OpenIdTab />)}
                        {self.state.tabName === 'scim' && (<ScimTab />)}
                        {self.state.tabName === 'authorization' && (<AuthorizationTab />)}
                    </div>
                </div>
            </div>
        </div>);
    }

    componentDidMount() {
        var self = this;
        var action = self.props.match.params.action;
        if (!action || (action !== 'openid' && action !== 'authorization' && action !== 'scim')) {
            action = 'openid';
        }

        self.setState({
            tabName: action
        });
    }
}

export default translate('common', { wait: process && !process.release })(withRouter(Logs));